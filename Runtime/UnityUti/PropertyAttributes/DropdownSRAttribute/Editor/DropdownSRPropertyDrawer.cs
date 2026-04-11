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
            var currentTypeIndex = string.IsNullOrEmpty(fullTypename)
                ? 0
                : _derivedTypes.FindIndex(t => t.type?.FullName == fullTypename);

            _mainVisualElement = new VisualElement();
            RecreateMainVisualElement(currentTypeIndex);
            return _mainVisualElement;
        }

        void RecreateMainVisualElement(int currentTypeIndex)
        {
            _mainVisualElement.Clear();
            var dropdown = CreateDropdown(currentTypeIndex);

            if (HasVisibleChildren(_property))
                _mainVisualElement.Add(CreateFoldout(dropdown));
            else
                _mainVisualElement.Add(CreateLabelRow(dropdown));
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

        VisualElement CreateLabelRow(DropdownField dropdown)
        {
            var row = new VisualElement
            {
                style = { flexDirection = FlexDirection.Row, alignItems = Align.Center }
            };
            var label = new Label(_property.displayName);
            label.AddToClassList("unity-base-field__label");
            row.Add(label);
            row.Add(dropdown);
            return row;
        }

        #endregion

        #region [Methods: Callbacks and Logic]

        static List<(Type type, string name)> GetDerivedTypeNames(Type type)
        {
            var types = TypeCache.GetTypesDerivedFrom(type)
                .Where(t => !t.IsAbstract)
                .Select(t => (t, t.FullName))
                .ToList();
            types.Insert(0, (null, "Null"));
            return types;
        }

        static bool HasVisibleChildren(SerializedProperty property)
        {
            var iterator = property.Copy();
            var end = iterator.GetEndProperty();
            return iterator.NextVisible(true) && !SerializedProperty.EqualContents(iterator, end);
        }

        static string ExcludeAssemblyName(string managedReferenceFullTypename)
        {
            var spaceIndex = managedReferenceFullTypename.IndexOf(' ');
            var typeName = spaceIndex >= 0
                ? managedReferenceFullTypename.Substring(spaceIndex + 1)
                : managedReferenceFullTypename;
            return typeName.Replace('/', '+');
        }

        void Dropdown_OnValueChanged(ChangeEvent<string> evt)
        {
            if (evt.newValue == evt.previousValue)
                return;

            var currentTypeIndex = _derivedTypes.FindIndex(t => t.name == evt.newValue);
            if (currentTypeIndex >= 0)
            {
                var selectedType = _derivedTypes[currentTypeIndex].type;
                _property.managedReferenceValue = selectedType != null ? Activator.CreateInstance(selectedType) : null;
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

        #region [IMGUI]

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.ManagedReference)
                return EditorGUIUtility.singleLineHeight;

            var height = EditorGUIUtility.singleLineHeight;
            if (property.isExpanded && HasVisibleChildren(property))
            {
                var iterator = property.Copy();
                var end = iterator.GetEndProperty();
                iterator.NextVisible(true);
                while (!SerializedProperty.EqualContents(iterator, end))
                {
                    height += EditorGUI.GetPropertyHeight(iterator, true) + EditorGUIUtility.standardVerticalSpacing;
                    if (!iterator.NextVisible(false))
                        break;
                }
            }

            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.ManagedReference)
            {
                DrawIMGUIErrorField(position, property, label);
                return;
            }

            DrawIMGUIMainField(position, property, label);
        }

        void DrawIMGUIMainField(Rect position, SerializedProperty property, GUIContent label)
        {
            _derivedTypes ??= GetDerivedTypeNames(fieldInfo.FieldType);

            var fullTypename = ExcludeAssemblyName(property.managedReferenceFullTypename);
            var currentTypeIndex = string.IsNullOrEmpty(fullTypename)
                ? 0
                : _derivedTypes.FindIndex(t => t.type?.FullName == fullTypename);
            var hasChildren = HasVisibleChildren(property);

            var labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
            var dropdownRect = new Rect(position.x + EditorGUIUtility.labelWidth, position.y, position.width - EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);

            if (hasChildren)
                property.isExpanded = EditorGUI.Foldout(labelRect, property.isExpanded, label, true);
            else
                EditorGUI.LabelField(labelRect, label);

            EditorGUI.BeginChangeCheck();
            var newIndex = EditorGUI.Popup(dropdownRect, currentTypeIndex, _derivedTypes.Select(t => t.name).ToArray());
            if (EditorGUI.EndChangeCheck() && newIndex != currentTypeIndex && newIndex >= 0)
            {
                var selectedType = _derivedTypes[newIndex].type;
                property.managedReferenceValue = selectedType != null ? Activator.CreateInstance(selectedType) : null;
                property.serializedObject.ApplyModifiedProperties();
            }

            if (!hasChildren || !property.isExpanded)
                return;

            EditorGUI.indentLevel++;
            var y = position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            var iterator = property.Copy();
            var end = iterator.GetEndProperty();
            iterator.NextVisible(true);
            while (!SerializedProperty.EqualContents(iterator, end))
            {
                var h = EditorGUI.GetPropertyHeight(iterator, true);
                var childRect = new Rect(position.x, y, position.width, h);
                EditorGUI.PropertyField(childRect, iterator, true);
                y += h + EditorGUIUtility.standardVerticalSpacing;
                if (!iterator.NextVisible(false))
                    break;
            }
            EditorGUI.indentLevel--;
        }

        void DrawIMGUIErrorField(Rect position, SerializedProperty property, GUIContent label)
        {
            var warningContent = new GUIContent(
                EditorGUIUtility.IconContent("Warning").image,
                "Property must be a managed reference of a type with derived types");
            var iconRect = new Rect(position.x, position.y, 20, EditorGUIUtility.singleLineHeight);
            var propertyRect = new Rect(position.x + 20, position.y, position.width - 20, position.height);

            GUI.Label(iconRect, warningContent);
            EditorGUI.PropertyField(propertyRect, property, label, true);
        }

        #endregion
    }
}
