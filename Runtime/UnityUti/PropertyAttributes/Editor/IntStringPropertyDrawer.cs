using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PlugRMK.UnityUti.EditorUti
{
    [CustomPropertyDrawer(typeof(IntStringList.IntString))]
    public class IntStringPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var row = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    alignItems = Align.Center,
                }
            };

            var label = new Label(property.displayName)
            {
                style = { minWidth = 120, flexShrink = 0 }
            };

            var intField = new PropertyField(property.FindPropertyRelative("IntValue"), "")
            {
                style = { flexGrow = 1, flexShrink = 0, flexBasis = 0, marginRight = 2 }
            };

            var stringField = new PropertyField(property.FindPropertyRelative("StringValue"), "")
            {
                style = { flexGrow = 2, flexShrink = 0, flexBasis = 0 }
            };

            row.Add(label);
            row.Add(intField);
            row.Add(stringField);
            return row;
        }
    }
}