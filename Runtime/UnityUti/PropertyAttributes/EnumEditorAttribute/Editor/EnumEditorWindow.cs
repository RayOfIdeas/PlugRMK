using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PlugRMK.UnityUti.EditorUti
{
    public class EnumEditorWindow : EditorWindow
    {
        class EnumMember
        {
            public string Name;
            public int Value;
        }

        [SerializeField]
        string _enumTypeAssemblyName;
        Type _enumType;
        List<EnumMember> _enumMembers = new();
        UnityEngine.Object _enumFileObject;
        ScrollView _scrollView;
        Button _saveButton;
        bool _isDirty;

        #region [Methods: Window]

        public static void Open(Type enumType)
        {
            if (!enumType.IsEnum)
            {
                Debug.LogError($"Type {enumType} is not an enum.");
                return;
            }

            var window = GetWindow<EnumEditorWindow>(
                utility: true,
                title: $"Enum {enumType.Name}",
                focus: true
            );
            window.titleContent = new GUIContent($"Enum {enumType.Name}");
            window.SetEnum(enumType);
        }

        void OnEnable()
        {
            if (_enumType == null && !string.IsNullOrEmpty(_enumTypeAssemblyName))
            {
                var type = Type.GetType(_enumTypeAssemblyName);
                if (type != null)
                    SetEnum(type);
            }
        }

        #endregion

        #region [Methods: UI]

        public void CreateGUI()
        {
            if (_enumType == null)
                return;

            var root = rootVisualElement;
            root.Clear();
            root.style.paddingTop = 2;
            root.style.paddingLeft = 8;
            root.style.paddingRight = 8;
            root.style.paddingBottom = 8;

            root.Add(CreateEnumFileObjectField());
            root.Add(CreateColumHeadears());
            root.Add(CreateEnumMembersScrollView());
            root.Add(CreateAddMemberButton());
            root.Add(new VisualElement() { style = { flexGrow = 1 } });
            root.Add(CreateSaveButton());
        }

        VisualElement CreateEnumFileObjectField()
        {
            var objectField = new ObjectField("Script")
            {
                objectType = typeof(UnityEngine.Object),
                value = _enumFileObject
            };

            objectField.RegisterValueChangedCallback(EnumFileObject_OnChanged);
            return objectField;
        }

        VisualElement CreateColumHeadears()
        {
            var container = new VisualElement()
            {
                style =
                {
                    paddingTop = 4,
                    flexDirection = FlexDirection.Row,
                    alignItems = Align.Center
                }
            };

            var nameLabel = new Label("Name")
            {
                style =
                {
                    flexGrow = 1,
                    unityFontStyleAndWeight = FontStyle.Bold,
                    unityTextAlign = TextAnchor.MiddleCenter
                }
            };

            var valueLabel = new Label("Value")
            {
                style =
                {
                    width = 80,
                    unityFontStyleAndWeight = FontStyle.Bold,
                    unityTextAlign = TextAnchor.MiddleCenter
                }
            };

            var space = new VisualElement()
            {
                style =
                {
                    width = 26,
                    flexShrink = 0
                }
            };

            container.Add(nameLabel);
            container.Add(valueLabel);
            container.Add(space);
            return container;
        }

        VisualElement CreateEnumMembersScrollView()
        {
            _scrollView = new ScrollView()
            {
                style =
                {
                }
            };

            foreach (var member in _enumMembers)
                _scrollView.Add(CreateEnumMemberElement(member));

            return _scrollView;
        }

        VisualElement CreateEnumMemberElement(EnumMember member)
        {
            var container = new VisualElement()
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    alignItems = Align.Center
                }
            };

            var nameField = new TextField()
            {
                value = member.Name,
                style = { flexGrow = 1 }
            };
            nameField.RegisterValueChangedCallback(evt => { member.Name = evt.newValue; SetAsDirty(); });

            var valueField = new IntegerField()
            {
                value = member.Value,
                style =
                {
                    width = 60,
                    flexShrink = 0
                }
            };
            valueField.RegisterValueChangedCallback(evt => { member.Value = evt.newValue; SetAsDirty(); });

            var removeButton = new Button(() =>
            {
                _enumMembers.Remove(member);
                _scrollView.Remove(container);
                SetAsDirty();
            })
            {
                text = "X",
                style =
                {
                    width = 22,
                    height = EditorGUIUtility.singleLineHeight,
                    flexShrink = 0
                }
            };

            container.Add(nameField);
            container.Add(valueField);
            container.Add(removeButton);
            return container;
        }

        VisualElement CreateAddMemberButton()
        {
            var button = new Button(AddMember)
            {
                text = "Add",
                style =
                {
                    height = EditorGUIUtility.singleLineHeight,
                    flexShrink = 0,
                }
            };
            return button;
        }

        VisualElement CreateSaveButton()
        {
            _saveButton = new Button(SaveEnum)
            {
                text = "Save",
                style =
                {
                    height = EditorGUIUtility.singleLineHeight * 1.5f,
                    flexShrink = 0,
                }
            };

            return _saveButton;
        }

        #endregion

        #region [Methods: Dirty]

        void SetAsDirty()
        {
            if (_isDirty)
                return;

            _isDirty = true;
            if (_saveButton != null)
                _saveButton.style.backgroundColor = Color.seaGreen;
        }

        void ClearDirty()
        {
            _isDirty = false;
            if (_saveButton != null)
                _saveButton.style.backgroundColor = StyleKeyword.Null;
        }

        #endregion

        #region [Methods: Logic]

        void EnumFileObject_OnChanged(ChangeEvent<UnityEngine.Object> evt)
        {
            if (evt.newValue == null)
                return;
            var path = AssetDatabase.GetAssetPath(evt.newValue);
            var assemblyName = File.ReadAllText(path);
            var type = Type.GetType(assemblyName);
            if (type == null || !type.IsEnum)
            {
                Debug.LogError($"The selected file does not contain a valid enum type.");
                return;
            }
            SetEnum(type);
        }

        void SetEnum(Type enumType)
        {
            _enumType = enumType;
            _enumTypeAssemblyName = enumType.AssemblyQualifiedName;
            _enumMembers = new();
            var names = Enum.GetNames(enumType);
            var values = Enum.GetValues(enumType);
            for (var i = 0; i < names.Length; i++)
            {
                _enumMembers.Add(new EnumMember
                {
                    Name = names[i],
                    Value = (int)values.GetValue(i)
                });
            }
            _enumFileObject = FindEnumFileObject(enumType);
            CreateGUI();
        }

        static UnityEngine.Object FindEnumFileObject(Type type)
        {
            foreach (var guid in AssetDatabase.FindAssets($"{type.Name} t:script"))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (Path.GetFileNameWithoutExtension(path) == type.Name)
                    return AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
            }
            return null;
        }

        void AddMember()
        {
            var newMember = new EnumMember
            {
                Name = $"NewMember{_enumMembers.Count}",
                Value = _enumMembers.Count > 0 ? _enumMembers[^1].Value + 1 : 0
            };
            _enumMembers.Add(newMember);
            _scrollView.Add(CreateEnumMemberElement(newMember));
            SetAsDirty();
        }

        void SaveEnum()
        {
            if (_enumFileObject == null)
            {
                EditorUtility.DisplayDialog("Error", "No script file assigned.", "OK");
                return;
            }

            var path = Path.GetFullPath(AssetDatabase.GetAssetPath(_enumFileObject));
            var source = File.ReadAllText(path);

            // Capture the leading whitespace of the enum declaration line
            var declMatch = Regex.Match(source,
                $@"^([ \t]*).*\benum\s+{Regex.Escape(_enumType.Name)}\b",
                RegexOptions.Multiline);
            var enumIndent = declMatch.Success ? declMatch.Groups[1].Value : "";
            var memberIndent = enumIndent + "\t";

            // Replace the enum body
            var bodyPattern = $@"(\benum\s+{Regex.Escape(_enumType.Name)}(?:\s*:\s*\w+)?)\s*{{[^}}]*}}";
            var newSource = Regex.Replace(source, bodyPattern,
                $"$1\n{enumIndent}{{\n{BuildEnumBody(memberIndent)}{enumIndent}}}",
                RegexOptions.Singleline);

            if (newSource == source)
            {
                EditorUtility.DisplayDialog("Warning", "Could not locate the enum definition.", "OK");
                return;
            }

            File.WriteAllText(path, newSource);
            AssetDatabase.Refresh();
            ClearDirty();
        }

        string BuildEnumBody(string memberIndent)
        {
            var sb = new StringBuilder();
            foreach (var member in _enumMembers)
                sb.AppendLine($"{memberIndent}{member.Name} = {member.Value},");
            return sb.ToString();
        }

        #endregion
    }
}
