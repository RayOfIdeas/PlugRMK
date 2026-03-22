using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PlugRMK.UnityUti.EditorUti
{
    [CustomPropertyDrawer(typeof(DropdownSRAttribute))]
    public class DropdownSRPropertyDrawer : PropertyDrawer
    {
        List<(Type type, string name)> _derivedTypes;
        VisualElement _mainVisualElement;
        SerializedProperty _property;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            if (property.propertyType == SerializedPropertyType.ManagedReference)
                return CreateMainVisualElement(property);
            return CreateErrorField(property);
        }

        #region [Methods: Create Main Visual Element]

        VisualElement CreateMainVisualElement(SerializedProperty property)
        {
            _property = property;
            _derivedTypes = GetDerivedTypeNames(fieldInfo.FieldType);
            var fullTypename = ExcludeAssemblyName(property.managedReferenceFullTypename);
            var currentTypeIndex = _derivedTypes.FindIndex(t => t.type.FullName == fullTypename);

            _mainVisualElement = new VisualElement();
            RecreateMainVisualElement(currentTypeIndex);
            return _mainVisualElement;
        }

        void RecreateMainVisualElement(int currentTypeIndex)
        {
            _mainVisualElement.Clear();
            var dropdown = CreateDropdown(currentTypeIndex);
            _mainVisualElement.Add(CreateFoldout(dropdown));
        }

        DropdownField CreateDropdown(int currentTypeIndex)
        {
            var dropdown = new DropdownField(
                _derivedTypes.Select(t => t.name).ToList(),
                currentTypeIndex)
            {
                style =
                {
                    flexGrow = 1f,
                }
            };
            dropdown.RegisterValueChangedCallback(Dropdown_OnValueChanged);
            dropdown.RegisterCallback<PointerDownEvent>(e => e.StopPropagation());
            return dropdown;
        }

        Foldout CreateFoldout(DropdownField dropdown)
        {
            var foldout = new Foldout
            {
                text = _property.displayName,
                value = _property.isExpanded
            };

            foldout.RegisterValueChangedCallback(evt =>
            {
                _property.isExpanded = evt.newValue;
                _property.serializedObject.ApplyModifiedProperties();
            });

            var toggle = foldout.Q<Toggle>();
            toggle.Add(dropdown);
            var toggleInput = foldout.Q(className: "unity-foldout__input");
            toggleInput.AddToClassList("unity-base-field__label");
            toggleInput.style.flexGrow = .25f;
            toggleInput.style.paddingLeft = 0f;

            var iterator = _property.Copy();
            var end = iterator.GetEndProperty();
            if (!iterator.NextVisible(true))
                return foldout;

            while (!SerializedProperty.EqualContents(iterator, end))
            {
                var child = new PropertyField(iterator.Copy());
                foldout.Add(child);
                child.BindProperty(iterator.Copy());
                if (!iterator.NextVisible(false))
                    break;
            }
            return foldout;
        }

        #endregion

        #region [Methods: Callbacks and Logic]

        static List<(Type type, string name)> GetDerivedTypeNames(Type type)
        {
            return TypeCache.GetTypesDerivedFrom(type)
                .Where(t => !t.IsAbstract)
                .Select(t => (t, t.FullName))
                .ToList();
        }

        static string ExcludeAssemblyName(string managedReferenceFullTypename)
        {
            var spaceIndex = managedReferenceFullTypename.IndexOf(' ');
            return spaceIndex >= 0
                ? managedReferenceFullTypename.Substring(spaceIndex + 1)
                : managedReferenceFullTypename;
        }

        void Dropdown_OnValueChanged(ChangeEvent<string> evt)
        {
            if (evt.newValue == evt.previousValue)
                return;

            var currentTypeIndex = _derivedTypes.FindIndex(t => t.name == evt.newValue);
            if (currentTypeIndex >= 0)
            {
                _property.managedReferenceValue = Activator.CreateInstance(_derivedTypes[currentTypeIndex].type);
                _property.serializedObject.ApplyModifiedProperties();
                RecreateMainVisualElement(currentTypeIndex);
            }
        }

        #endregion

        #region [Methods: Create Error Field]

        static VisualElement CreateErrorField(SerializedProperty property)
        {
            var container = new VisualElement()
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    alignItems = Align.Center,
                }
            };
            var warningIcon = (Texture2D)EditorGUIUtility.IconContent("Warning").image;
            var waringImage = new Image
            {
                image = warningIcon,
                scaleMode = ScaleMode.ScaleToFit,
                style = { width = 16, height = 16, marginRight = 4 },
                tooltip = $"Property must be a managed reference of a type with derived types"
            };
            var propertyField = new PropertyField(property)
            {
                style = { flexGrow = 1, flexShrink = 0, flexBasis = 0 },
            };
            container.Add(waringImage);
            container.Add(propertyField);
            return container;
        }

        #endregion
    }
}
