using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace PlugRMK.UnityUti.Hext
{
    [InitializeOnLoad]
    public class HierarchyExt
    {
        const int BACKGROUND_WIDTH_PADDING = 60;
        const string STYLE_LIST_PATH = "Packages/com.rayofideas.plugrmk/Runtime/UnityUti/HierarchyExt/Editor/HierarchyExtStyleList.asset";

        static HierarchyExtStyleList _extStyleList;
        static HierarchyExtStyleList ExtStyleList
        {
            get
            {
                _extStyleList ??= AssetDatabase.LoadAssetAtPath<HierarchyExtStyleList>(STYLE_LIST_PATH);
                return _extStyleList;
            }
        }
        static bool _hierarchyHasFocus = false;
        static EditorWindow _hierarchyEditorWindow;
        static Dictionary<HierarchyExtStyle, GUIStyle> _cachedLabelStyles = new();

        static HierarchyExt()
        {
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyWindow;
            EditorApplication.update += OnEditorUpdate;
        }

        #region [Methods: Callbacks]

        static void OnEditorUpdate()
        {
            if (_hierarchyEditorWindow == null && AnyHierarchyOpened())
                _hierarchyEditorWindow = EditorWindow.GetWindow(Type.GetType("UnityEditor.SceneHierarchyWindow,UnityEditor"));
            _hierarchyHasFocus = EditorWindow.focusedWindow != null && EditorWindow.focusedWindow == _hierarchyEditorWindow;
        }

        static bool AnyHierarchyOpened()
        {
            return Resources.FindObjectsOfTypeAll<EditorWindow>()
                .Where(window => window.GetType().Name == "SceneHierarchyWindow")
                .Any();
        }

        static void OnHierarchyWindow(int instanceID, Rect selectionRect)
        {
            if (EditorUtility.InstanceIDToObject(instanceID) is GameObject go)
            {
                if (go.TryGetComponent<HierarchyExtGO>(out var hextGO))
                    Stylize(instanceID, selectionRect, go.name, hextGO.style);
                else if (TryGetStyleFromName(go.name, out var style))
                    Stylize(instanceID, selectionRect, go.name, style);
            }
        }
       
        #endregion

        #region [Methods: Stylize]

        static void Stylize(int instanceID, Rect selectionRect, string goName, HierarchyExtStyle style)
        {
            DrawRectAsBG(instanceID, selectionRect, style);
            DrawLabelName(selectionRect, goName, style);
            DrawIcon(selectionRect, style);
        }

        static bool TryGetStyleFromName(string name, out HierarchyExtStyle foundStyle)
        {
            if (ExtStyleList != null && ExtStyleList.styles.Count > 0)
            {
                for (int i = 0; i < ExtStyleList.styles.Count; i++)
                {
                    var style = ExtStyleList.styles[i];
                    if (name.StartsWith(style.token))
                    {
                        foundStyle = style;
                        return true;
                    }
                }
            }

            foundStyle = null;
            return false;
        }

        #endregion

        #region [Methods: Draw Rect as Background]

        static void DrawRectAsBG(int instanceID, Rect selectionRect, HierarchyExtStyle style)
        {
            var (isSelected, isHovered, isWindowFocused) = GetSelectionState(instanceID, selectionRect);
            var bgColor = style.isCustomBG
                ? style.GetBGColor(isSelected, isHovered, isWindowFocused)
                : GetEditorBGColor(isSelected, isHovered, isWindowFocused);

            var bgOffsetX = style.isCustomBGOffset
                ? style.bgOffset.x
                : style.icon != null ? 0 : HierarchyExtStyle.bgOffsetDefault.x;

            var bgOffsetY = style.isCustomBGOffset
                ? style.bgOffset.y
                : HierarchyExtStyle.bgOffsetDefault.y;

            var bgRect = new Rect(
                selectionRect.x + bgOffsetX,
                selectionRect.y + bgOffsetY,
                selectionRect.width + BACKGROUND_WIDTH_PADDING,
                selectionRect.height);

            EditorGUI.DrawRect(bgRect, bgColor);
        }

        static (bool isSelected, bool isHovered, bool isWindowFocused) GetSelectionState(int instanceID, Rect selectionRect)
        {
            var isSelected = Selection.instanceIDs.Contains(instanceID);
            var isHovered = selectionRect.Contains(Event.current.mousePosition);
            var isWindowFocused = _hierarchyHasFocus;
            return (isSelected, isHovered, isWindowFocused);
        }

        static Color GetEditorBGColor(bool isSelected, bool isHovered, bool isWindowFocused)
        {
            if (isSelected)
            {
                if (isWindowFocused)
                {
                    return EditorGUIUtility.isProSkin ? UnityEditorColorUtility.k_selectedProColor : UnityEditorColorUtility.k_selectedColor;
                }
                else
                {
                    return EditorGUIUtility.isProSkin ? UnityEditorColorUtility.k_selectedUnFocusedProColor : UnityEditorColorUtility.k_selectedUnFocusedColor;
                }
            }
            else if (isHovered)
            {
                return EditorGUIUtility.isProSkin ? UnityEditorColorUtility.k_hoveredProColor : UnityEditorColorUtility.k_hoveredColor;
            }
            else
            {
                return EditorGUIUtility.isProSkin ? UnityEditorColorUtility.k_defaultProColor : UnityEditorColorUtility.k_defaultcolor;
            }
        }

        #endregion

        #region [Methods: Draw Label Name]

        static void DrawLabelName(Rect selectionRect, string goName, HierarchyExtStyle style)
        {
            var newName = GetLabelName(goName, style);
            var newStyle = GetLabelGUIStyle(style);
            EditorGUI.LabelField(selectionRect, newName, newStyle);
        }

        static string GetLabelName(string goName, HierarchyExtStyle style)
        {
            return !string.IsNullOrWhiteSpace(style.labelName)
                ? style.labelName
                : style.removeTokenFromLabel
                    ? goName.Replace(style.token, "")
                    : goName;
        }

        static GUIStyle GetLabelGUIStyle(HierarchyExtStyle style)
        {
            if (_cachedLabelStyles.TryGetValue(style, out var cachedStyle))
            {
                return cachedStyle;
            }
            else
            {
                var newStyle = CreateLabelGUIStyle(style);
                _cachedLabelStyles[style] = newStyle;
                return newStyle;
            }
        }

        static GUIStyle CreateLabelGUIStyle(HierarchyExtStyle style)
        {
            return new GUIStyle
            {
                alignment = style.textAlignment,
                fontStyle = style.fontStyle,
                normal = new GUIStyleState()
                {
                    textColor = style.textColor,
                },
                padding = new RectOffset(style.textOffset.x, style.textOffset.y, 0, 0)
            };
        }

        #endregion

        #region [Methods: Draw Icon]

        static void DrawIcon(Rect selectionRect, HierarchyExtStyle style)
        {
            if (style.icon != null)
                EditorGUI.LabelField(selectionRect, new GUIContent(style.icon.texture));
        }

        #endregion

        #region [Methods: MenuItem]

        [MenuItem("GameObject/PlugRMK/Instantiate All Hierarchy Ext List")]
        public static void CreateAllHierarchyExtList()
        {
            if (ExtStyleList != null)
                ExtStyleList.InstantiateToScene();
            else
                Debug.LogWarning("HierarchyExtStyleList is not found or loaded yet");
        }

        [MenuItem("Tools/Hierarchy Ext Style List")]
        public static void OpenHierarchyExtStyleList()
        {
            if (ExtStyleList != null)
                Selection.activeObject = ExtStyleList;
            else
                Debug.LogWarning("HierarchyExtStyleList is not found or loaded yet");
        }

        #endregion
    }
}
