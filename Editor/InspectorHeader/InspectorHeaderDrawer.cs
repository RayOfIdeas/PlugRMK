using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PlugRMK.UnityUti.EditorUti
{
    [InitializeOnLoad]
    public static class InspectorHeaderDrawer
    {
        static readonly Action<Editor> onDrawToolbar;
        const float LEFT_PADDING = 40f;

        static InspectorHeaderDrawer()
        {
            onDrawToolbar = delegate { };
            var methods = TypeCache.GetMethodsWithAttribute<InspectorHeaderButtonAttribute>();
            foreach (var method in methods)
                if (method.IsStatic)
                    onDrawToolbar += (Action<Editor>)Delegate.CreateDelegate(typeof(Action<Editor>), method);

            Editor.finishedDefaultHeaderGUI += OnFinishedHeaderGUI;
        }

        static void OnFinishedHeaderGUI(Editor editor)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(LEFT_PADDING);
            onDrawToolbar(editor);
            GUILayout.EndHorizontal();
        }

        public static bool DrawButton(string iconName, float width = 48f, float height = 18f)
        {
            return GUILayout.Button(EditorGUIUtility.IconContent(iconName), GUILayout.Width(width), GUILayout.Height(height));
        }

        public static void DrawLabel(string iconName, float width = 20f, float height = 18f)
        {
            GUILayout.Label(EditorGUIUtility.IconContent(iconName), GUILayout.Width(width), GUILayout.Height(height));
        }
    }
}
