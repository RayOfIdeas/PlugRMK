using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PlugRMK.UnityUti.EditorUti
{ 
    [CustomPropertyDrawer(typeof(LockAttribute))]
    public class LockPropertyDrawer : PropertyDrawer
    {
        const float LOCK_ICON_WIDTH = 25;
        const float LOCK_ICON_SPACE = 15;
        bool isLocked = true;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            isLocked = UnlockIfFieldEmpty();
            var field = CreateField(position, property, label);
            var lockButton = CreateLockButton(position, field);

            bool UnlockIfFieldEmpty()
            {
                if (property.propertyType == SerializedPropertyType.ObjectReference && property.objectReferenceValue == null)
                    return false;
                if (property.propertyType == SerializedPropertyType.String && string.IsNullOrEmpty(property.stringValue))
                    return false;

                return isLocked;
            }

            Rect CreateField(Rect position, SerializedProperty property, GUIContent label)
            {
                EditorGUI.BeginDisabledGroup(isLocked);
                var saveKeyFieldRect = new Rect(position);
                saveKeyFieldRect.width -= LOCK_ICON_WIDTH;
                EditorGUI.PropertyField(saveKeyFieldRect, property, label, true);
                EditorGUI.EndDisabledGroup();
                return saveKeyFieldRect;
            }

            Rect CreateLockButton(Rect position, Rect saveKeyFieldRect)
            {
                var buttonRect = new Rect(position);
                buttonRect.width = LOCK_ICON_WIDTH;
                buttonRect.x = saveKeyFieldRect.width + LOCK_ICON_SPACE;

                if (isLocked && GUI.Button(buttonRect, EditorGUIUtility.IconContent("LockIcon-On")))
                {
                    isLocked = false;
                }
                else if (!isLocked && GUI.Button(buttonRect, EditorGUIUtility.IconContent("LockIcon")))
                {
                    isLocked = true;
                }

                return buttonRect;
            }
        }
    }
}
