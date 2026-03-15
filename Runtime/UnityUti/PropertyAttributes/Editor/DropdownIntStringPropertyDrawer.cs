using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PlugRMK.UnityUti.EditorUti
{
    [CustomPropertyDrawer(typeof(DropdownIntStringAttribute))]
    public class DropdownIntStringPropertyDrawer : PropertyDrawer
    {
        static readonly Dictionary<IntStringList, EditorWindow> _openWindows = new();

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var dropdownIntString = (DropdownIntStringAttribute)attribute;
            if (property.propertyType == SerializedPropertyType.Integer &&
                TryFindIntStringList(dropdownIntString.ListName, out var list))
            {
                return CreateMainVisualElement(property, list);
            }
            return CreateErrorField(property, dropdownIntString);
        }

        VisualElement CreateMainVisualElement(SerializedProperty property, IntStringList list)
        {
            var strings = list.IntStrings.Select(x => x.StringValue).ToList();
            strings.Add("<None>");
            var ints = list.IntStrings.Select(x => x.IntValue).ToList();

            var container = CreateContainer();
            var intField = CreateIntField(property);
            var dropdown = CreateDropdown(property, strings, ints);

            intField.RegisterValueChangeCallback(_ =>
                UpdateDropdownCurrentIndex(dropdown, property.intValue, ints));

            var listSO = new SerializedObject(list);
            container.userData = listSO;
            container.TrackSerializedObjectValue(listSO, _ =>
                UpdateDropdown(property, list, strings, ints, dropdown));

            container.Add(intField);
            container.Add(dropdown);
            container.Add(CreateShowButton(list));
            return container;
        }

        static bool TryFindIntStringList(string listName, out IntStringList list)
        {
            var guids = AssetDatabase.FindAssets($"{listName} t:IntStringList");
            if (guids.Length > 0)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
                list = AssetDatabase.LoadAssetAtPath<IntStringList>(assetPath);
                if (list != null && list.IntStrings != null)
                    return true;
            }
            list = null;
            return false;
        }

        static VisualElement CreateContainer()
        {
            return new VisualElement()
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    alignItems = Align.Center,
                }
            };
        }

        static PropertyField CreateIntField(SerializedProperty property)
        {
            return new PropertyField(property)
            {
                style = { flexGrow = 7, flexShrink = 0, flexBasis = 0, marginRight = 2 }
            };
        }

        static DropdownField CreateDropdown(SerializedProperty property, List<string> strings, List<int> ints)
        {
            var currentIndex = ints.IndexOf(property.intValue);
            if (currentIndex < 0)
                currentIndex = ints.Count;

            var dropdown = new DropdownField(strings, currentIndex)
            {
                style = { flexGrow = 5, flexShrink = 0, flexBasis = 0 }
            };

            dropdown.RegisterValueChangedCallback(evt =>
                Dropdown_OnValueChanged(property, strings, ints, dropdown, evt));

            return dropdown;
        }

        static void Dropdown_OnValueChanged(
            SerializedProperty property,
            List<string> strings,
            List<int> ints,
            DropdownField dropdown,
            ChangeEvent<string> evt)
        {
            var selectedIndex = strings.IndexOf(evt.newValue);
            if (selectedIndex >= 0 && selectedIndex < ints.Count)
            {
                property.intValue = ints[selectedIndex];
                property.serializedObject.ApplyModifiedProperties();
            }
            else
            {
                if (!string.IsNullOrEmpty(evt.previousValue))
                    dropdown.SetValueWithoutNotify(evt.previousValue);
            }
        }

        static void UpdateDropdown(
            SerializedProperty property,
            IntStringList list,
            List<string> strings,
            List<int> ints,
            DropdownField dropdown)
        {
            strings.Clear();
            strings.AddRange(list.IntStrings.Select(x => x.StringValue));
            strings.Add("<None>");
            ints.Clear();
            ints.AddRange(list.IntStrings.Select(x => x.IntValue));
            dropdown.choices = strings;
            UpdateDropdownCurrentIndex(dropdown, property.intValue, ints);
        }

        static void UpdateDropdownCurrentIndex(DropdownField dropdown, int intValue, List<int> ints)
        {
            var currentIndex = ints.IndexOf(intValue);
            if (currentIndex < 0)
                currentIndex = ints.Count;
            var targetValue = dropdown.choices[currentIndex];
            if (dropdown.value != targetValue)
                dropdown.SetValueWithoutNotify(targetValue);
        }

        static VisualElement CreateShowButton(IntStringList list)
        {
            var icon = (Texture2D)EditorGUIUtility.IconContent("d_ScriptableObject Icon").image;
            var showButton = new Button(() => OpenInspector(list))
            {
                style =
                {
                    width = 22,
                    height = EditorGUIUtility.singleLineHeight,
                    flexShrink = 0,
                    paddingLeft = 1,
                    paddingRight = 1,
                    paddingTop = 1,
                    paddingBottom = 1
                }
            };
            showButton.Add(new Image
            {
                image = icon,
                scaleMode = ScaleMode.ScaleToFit,
                pickingMode = PickingMode.Ignore,
                style = { flexGrow = 1 }
            });
            return showButton;
        }

        static void OpenInspector(IntStringList list)
        {
            if (_openWindows.TryGetValue(list, out var existing) && existing != null)
            {
                existing.Focus();
                return;
            }

            var window = EditorWindow.CreateInstance<EditorWindow>();
            window.titleContent = new GUIContent(list.name);
            var inspector = new InspectorElement(list);
            window.rootVisualElement.Add(inspector);
            window.ShowUtility();
        }

        static VisualElement CreateErrorField(SerializedProperty property, DropdownIntStringAttribute attribute)
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
                tooltip = $"Property must be an integer and '{attribute.ListName}' must be an IntStringList asset"
            };
            var propertyField = new PropertyField(property)
            {
                style = { flexGrow = 1, flexShrink = 0, flexBasis = 0 },
            };
            container.Add(waringImage);
            container.Add(propertyField);
            return container;
        }
    }
}