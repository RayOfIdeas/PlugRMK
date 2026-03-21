using UnityEditor;
using UnityEngine;

namespace PlugRMK.UnityUti.EditorUti
{
    [CustomPropertyDrawer(typeof(DisableAttribute))]
    public class DisablePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.PropertyField(position, property, label, true);
            EditorGUI.EndDisabledGroup();
        }
    }
}