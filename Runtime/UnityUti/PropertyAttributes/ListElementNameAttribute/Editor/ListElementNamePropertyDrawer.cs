using UnityEditor;
using UnityEngine;

namespace PlugRMK.UnityUti.EditorUti
{
    [CustomPropertyDrawer(typeof(ListElementNameAttribute))]
    public class ListElementNamePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var listElementName = (ListElementNameAttribute)attribute;

            if (IsArrayProperty(property))
            {
                DrawArray(position, property, label, listElementName.ElementName);
                return;
            }

            label.text = GetElementLabel(property, listElementName.ElementName, label.text);
            EditorGUI.PropertyField(position, property, label, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!IsArrayProperty(property))
                return EditorGUI.GetPropertyHeight(property, label, true);

            float height = EditorGUIUtility.singleLineHeight;
            if (!property.isExpanded)
                return height;

            height += EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight;

            for (int i = 0; i < property.arraySize; i++)
            {
                height += EditorGUIUtility.standardVerticalSpacing;
                height += EditorGUI.GetPropertyHeight(property.GetArrayElementAtIndex(i), GUIContent.none, true);
            }

            return height;
        }

        static bool IsArrayProperty(SerializedProperty property)
        {
            return property.isArray && property.propertyType != SerializedPropertyType.String;
        }

        static void DrawArray(Rect position, SerializedProperty property, GUIContent label, string elementName)
        {
            EditorGUI.BeginProperty(position, label, property);

            float y = position.y;
            var foldoutRect = new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight);
            property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, label, true);
            y += EditorGUIUtility.singleLineHeight;

            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;

                y += EditorGUIUtility.standardVerticalSpacing;
                var sizeRect = new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight);
                EditorGUI.BeginChangeCheck();
                int newSize = EditorGUI.DelayedIntField(sizeRect, "Size", property.arraySize);
                if (EditorGUI.EndChangeCheck() && newSize >= 0)
                    property.arraySize = newSize;
                y += EditorGUIUtility.singleLineHeight;

                for (int i = 0; i < property.arraySize; i++)
                {
                    var element = property.GetArrayElementAtIndex(i);
                    var elementLabel = new GUIContent(GetElementLabel(element, elementName, $"Element {i}"));
                    y += EditorGUIUtility.standardVerticalSpacing;
                    float elementHeight = EditorGUI.GetPropertyHeight(element, elementLabel, true);
                    var elementRect = new Rect(position.x, y, position.width, elementHeight);
                    EditorGUI.PropertyField(elementRect, element, elementLabel, true);
                    y += elementHeight;
                }

                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        static string GetElementLabel(SerializedProperty element, string elementName, string fallback)
        {
            var elementProp = element.FindPropertyRelative(elementName);
            var displayValue = GetElementDisplayValue(elementProp);
            return displayValue ?? fallback;
        }

        static string GetElementDisplayValue(SerializedProperty elementProp)
        {
            if (elementProp == null)
                return null;

            return elementProp.propertyType switch
            {
                SerializedPropertyType.Integer => elementProp.intValue.ToString(),
                SerializedPropertyType.String => string.IsNullOrEmpty(elementProp.stringValue) ? "(empty)" : elementProp.stringValue,
                SerializedPropertyType.Enum => elementProp.enumDisplayNames[elementProp.enumValueIndex],
                SerializedPropertyType.Boolean => elementProp.boolValue.ToString(),
                _ => null,
            };
        }
    }
}