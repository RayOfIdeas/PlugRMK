using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PlugRMK.UnityUti.EditorUti
{
    [CustomPropertyDrawer(typeof(Sprite))]
    public class SpritePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // Store the original value to detect changes
            Object originalValue = property.objectReferenceValue;

            // Check if we have mixed values across multiple selected objects
            EditorGUI.showMixedValue = property.hasMultipleDifferentValues;

            // First ObjectField (smaller, with label)
            Rect fieldRect1 = new Rect(
                position.x,
                position.y + EditorGUIUtility.singleLineHeight * .5f,
                position.width - EditorGUIUtility.singleLineHeight * 2,
                EditorGUIUtility.singleLineHeight
            );

            Object newValue1 = EditorGUI.ObjectField(
                fieldRect1,
                label,
                property.objectReferenceValue,
                typeof(Sprite),
                false);

            // Second ObjectField (larger, without label - for preview)
            Rect fieldRect2 = new Rect(
                position.x,
                position.y,
                position.width,
                position.height
            );

            Object newValue2 = EditorGUI.ObjectField(
                fieldRect2,
                GUIContent.none, // Use GUIContent.none instead of " "
                property.objectReferenceValue,
                typeof(Sprite),
                false);

            // Only apply changes if the value actually changed and we're not showing mixed values
            // Use the value from whichever field was actually changed
            Object finalValue = property.objectReferenceValue;

            if (newValue1 != originalValue)
            {
                finalValue = newValue1;
            }
            else if (newValue2 != originalValue)
            {
                finalValue = newValue2;
            }

            // Only assign if there's actually a change
            if (finalValue != originalValue)
            {
                property.objectReferenceValue = finalValue;
            }

            // Reset mixed value display
            EditorGUI.showMixedValue = false;

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 2;
        }
    }
}
