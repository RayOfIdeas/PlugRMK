using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PlugRMK.UnityUti.EditorUti
{
    public class EnumDiffWindow : EditorWindow
    {
        public struct MemberDiff
        {
            public string OriginalName;
            public int OriginalValue;
            public string Name;
            public int Value;

            public bool IsNew => string.IsNullOrEmpty(OriginalName);
            public bool IsNameChanged => !IsNew && OriginalName != Name;
            public bool IsValueChanged => !IsNew && OriginalValue != Value;
            public bool IsChanged => IsNameChanged || IsValueChanged;
        }

        Type _enumType;
        UnityEngine.Object _enumFileObject;
        List<MemberDiff> _diffs;
        Action _onSave;

        VisualElement _refsContainer;
        Label _searchStatusLabel;
        Label _refsHeaderLabel;
        CancellationTokenSource _searchCts;

        public static void Open(
            Type enumType,
            UnityEngine.Object enumFileObject,
            List<MemberDiff> diffs,
            Action onSave)
        {
            foreach (var existing in Resources.FindObjectsOfTypeAll<EnumDiffWindow>())
                existing.Close();

            var window = CreateInstance<EnumDiffWindow>();
            window.titleContent = new GUIContent("Enum Diff");
            window.minSize = new Vector2(340, 280);
            window.maxSize = new Vector2(340, 4096);
            window._enumType = enumType;
            window._enumFileObject = enumFileObject;
            window._diffs = diffs;
            window._onSave = onSave;
            window.ShowUtility();
        }

        public void CreateGUI()
        {
            if (_enumType == null || _diffs == null)
                return;

            var root = rootVisualElement;
            root.Clear();
            root.style.paddingTop = 4;
            root.style.paddingLeft = 8;
            root.style.paddingRight = 8;
            root.style.paddingBottom = 8;

            var scrollView = new ScrollView();
            scrollView.style.flexGrow = 1;

            // --- Diff columns ---
            var headerRow = new VisualElement()
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    marginBottom = 4
                }
            };
            headerRow.Add(new Label("Before")
            {
                style =
                {
                    flexGrow = 1,
                    unityFontStyleAndWeight = FontStyle.Bold
                }
            });
            headerRow.Add(new Label("After")
            {
                style =
                {
                    flexGrow = 1,
                    unityFontStyleAndWeight = FontStyle.Bold
                }
            });
            scrollView.Add(headerRow);

            foreach (var diff in _diffs)
            {
                var row = new VisualElement()
                {
                    style = { flexDirection = FlexDirection.Row }
                };

                var beforeLabel = new Label(diff.IsNew ? "" : $"{diff.OriginalName} = {diff.OriginalValue}")
                {
                    style =
                    {
                        flexGrow = 1,
                        flexBasis = 0,
                        unityTextAlign = TextAnchor.UpperLeft,
                        overflow = Overflow.Hidden
                    }
                };

                var afterLabel = new Label($"{diff.Name} = {diff.Value}")
                {
                    style =
                    {
                        flexGrow = 1,
                        flexBasis = 0,
                        unityTextAlign = TextAnchor.UpperLeft,
                        overflow = Overflow.Hidden
                    }
                };

                if (diff.IsChanged || diff.IsNew)
                    afterLabel.style.color = Color.yellow;

                row.Add(beforeLabel);
                row.Add(afterLabel);
                scrollView.Add(row);
            }

            // --- Separator ---
            scrollView.Add(new VisualElement()
            {
                style =
                {
                    height = 1,
                    marginTop = 8,
                    marginBottom = 8,
                    backgroundColor = new Color(0.3f, 0.3f, 0.3f)
                }
            });

            // --- Referencing Scripts ---
            _refsHeaderLabel = new Label("Referencing Scripts")
            {
                style =
                {
                    unityFontStyleAndWeight = FontStyle.Bold,
                    marginBottom = 4
                }
            };
            scrollView.Add(_refsHeaderLabel);

            _refsContainer = new VisualElement();
            scrollView.Add(_refsContainer);

            _searchStatusLabel = new Label("Searching...")
            {
                style = { color = new Color(0.6f, 0.6f, 0.6f) }
            };
            scrollView.Add(_searchStatusLabel);

            root.Add(scrollView);

            // --- Save button ---
            root.Add(new VisualElement() { style = { height = 6 } });
            root.Add(new Button(OnSaveClicked)
            {
                text = "Save",
                style =
                {
                    height = EditorGUIUtility.singleLineHeight * 1.5f,
                    flexShrink = 0
                }
            });

            StartSearch();
        }

        void OnSaveClicked()
        {
            if (_onSave == null)
            {
                EditorUtility.DisplayDialog("Error", "Save action is no longer valid. Please close and reopen the editor.", "OK");
                return;
            }
            _searchCts?.Cancel();
            _onSave.Invoke();
            Close();
        }

        async void StartSearch()
        {
            _searchCts?.Cancel();
            _searchCts = new CancellationTokenSource();
            var token = _searchCts.Token;

            try
            {
                var renames = _diffs.Where(d => !d.IsNew && d.IsNameChanged).ToList();
                if (renames.Count == 0)
                {
                    if (_searchStatusLabel != null)
                        _searchStatusLabel.text = "No renamed members — no references to update.";
                    return;
                }

                if (_refsHeaderLabel != null)
                    _refsHeaderLabel.text = "Referencing Scripts (searching...)";

                var dataPath = Application.dataPath;
                var enumTypeName = _enumType.Name;
                var savedFilePath = _enumFileObject != null
                    ? Path.GetFullPath(AssetDatabase.GetAssetPath(_enumFileObject))
                    : null;

                var results = await Task.Run(() =>
                {
                    var found = new List<(string path, List<int> lines)>();
                    string[] csFiles;
                    try
                    {
                        csFiles = Directory.GetFiles(dataPath, "*.cs", SearchOption.AllDirectories);
                    }
                    catch
                    {
                        return found;
                    }

                    foreach (var file in csFiles)
                    {
                        if (token.IsCancellationRequested)
                            return found;

                        var isEnumFile = savedFilePath != null &&
                            string.Equals(Path.GetFullPath(file), savedFilePath, StringComparison.OrdinalIgnoreCase);

                        string content;
                        try { content = File.ReadAllText(file); }
                        catch { continue; }

                        var matchedLines = new List<int>();
                        var fileLines = content.Split('\n');
                        for (var i = 0; i < fileLines.Length; i++)
                        {
                            foreach (var rename in renames)
                            {
                                var pattern = isEnumFile
                                    ? $@"\b{Regex.Escape(rename.OriginalName)}\s*="
                                    : $@"\b{Regex.Escape(enumTypeName)}\.{Regex.Escape(rename.OriginalName)}\b";

                                if (Regex.IsMatch(fileLines[i], pattern))
                                {
                                    matchedLines.Add(i + 1);
                                    break;
                                }
                            }
                        }

                        if (matchedLines.Count > 0)
                            found.Insert(isEnumFile ? 0 : found.Count, (file, matchedLines));
                    }

                    return found;
                });

                // Back on main thread
                if (token.IsCancellationRequested)
                    return;

                if (_refsContainer == null || _searchStatusLabel == null)
                    return;

                if (_refsHeaderLabel != null)
                    _refsHeaderLabel.text = "Referencing Scripts";

                _searchStatusLabel.text = results.Count == 0 ? "No referencing scripts found." : "";

                var dataDirNorm = Path.GetDirectoryName(dataPath)!.Replace('\\', '/').TrimEnd('/') + '/';
                foreach (var result in results)
                {
                    var filePath = result.path;
                    var lines = result.lines;
                    var fileNorm = filePath.Replace('\\', '/');
                    if (!fileNorm.StartsWith(dataDirNorm, StringComparison.OrdinalIgnoreCase))
                        continue;

                    var assetPath = fileNorm.Substring(dataDirNorm.Length);
                    var obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath);
                    if (obj == null)
                        continue;

                    var row = new VisualElement()
                    {
                        style =
                        {
                            flexDirection = FlexDirection.Row,
                            alignItems = Align.Center,
                            marginBottom = 2
                        }
                    };

                    var objField = new ObjectField()
                    {
                        objectType = typeof(UnityEngine.Object),
                        value = obj,
                        style = { flexGrow = 1 }
                    };
                    objField.SetEnabled(false);

                    row.Add(objField);
                    row.Add(new Label(string.Join(", ", lines))
                    {
                        style =
                        {
                            color = new Color(0.6f, 0.6f, 0.6f),
                            marginLeft = 6,
                            flexShrink = 0
                        }
                    });

                    _refsContainer.Add(row);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[EnumDiffWindow] Search failed: {e.Message}");
                if (_searchStatusLabel != null)
                    _searchStatusLabel.text = "Search failed. See console for details.";
            }
        }
    }
}
