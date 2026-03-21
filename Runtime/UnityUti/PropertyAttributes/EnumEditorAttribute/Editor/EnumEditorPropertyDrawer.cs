using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PlugRMK.UnityUti.EditorUti
{
    [CustomPropertyDrawer(typeof(EnumEditorAttribute))]
    public class EnumEditorPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            if (property.propertyType == SerializedPropertyType.Enum)
                return CreateMainVisualElement(property, fieldInfo.FieldType);
            return CreateErrorField(property);
        }

        static VisualElement CreateMainVisualElement(SerializedProperty property, Type enumType)
        {
            var container = new VisualElement()
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    alignItems = Align.Center
                }
            };

            var propertyField = new PropertyField(property)
            {
                style = { flexGrow = 1 },
            };

            var openEditorButton = new Button(() => OpenEditorButton_OnClicked(enumType))
            {
                style =
                {
                    flexShrink = 0,
                    width = 22,
                    height = EditorGUIUtility.singleLineHeight,
                    paddingTop = 1,
                    paddingBottom = 1,
                    paddingLeft = 1,
                    paddingRight = 1,
                    backgroundColor = Color.clear
                }
            };

            var gearIcon = (Texture2D)EditorGUIUtility.IconContent("d_settings").image;
            openEditorButton.Add(new Image
            {
                image = gearIcon,
                scaleMode = ScaleMode.ScaleToFit,
                pickingMode = PickingMode.Ignore,
                style = { flexGrow = 1 },
            });

            container.Add(propertyField);
            container.Add(openEditorButton);
            return container;
        }

        static void OpenEditorButton_OnClicked(Type enumType)
        {
            EnumEditorWindow.Open(enumType);
        }

        static VisualElement CreateErrorField(SerializedProperty property)
        {
            var container = new VisualElement()
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    alignItems = Align.Center
                }
            };

            var errorLabel = new Label($"Error: {property.name} is not an enum.");
            errorLabel.style.color = Color.red;

            var propertyField = new PropertyField(property)
            {
                style = { flexGrow = 1, flexShrink = 0, flexBasis = 0 },
            };

            container.Add(errorLabel);
            container.Add(propertyField);
            return container;
        }
    }
}
