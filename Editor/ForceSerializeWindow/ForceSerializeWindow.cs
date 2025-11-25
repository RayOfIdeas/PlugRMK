using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PlugRMK.UnityUti.EditorUti
{
    public class ForceSerializeWindow : EditorWindow
    {
        [SerializeField]
        VisualTreeAsset visualTreeAsset = default;

        [MenuItem("Tools/Force Serialize")]
        public static void ShowExample()
        {
            var wnd = GetWindow<ForceSerializeWindow>();
            wnd.titleContent = new GUIContent("Force Serialize");
        }

        public void CreateGUI()
        {
            rootVisualElement.Add(visualTreeAsset.Instantiate());
            var keywordField = rootVisualElement.Q<TextField>("keywordField");
            var serializeButton = rootVisualElement.Q<Button>("serializeButton");

            serializeButton.clicked += () =>
            {
                if (!string.IsNullOrEmpty(keywordField.value) || DisplayDialogAboutSerializingAll())
                    ForceSerialize(keywordField.value);
            };
        }

        static bool DisplayDialogAboutSerializingAll()
        {
            return EditorUtility.DisplayDialog(
                title: "Force Serialize All Assets",
                message: "You are about to force serialize ALL assets in the project which may take a while.\nDo you want to proceed?",
                ok: "Yes",
                cancel: "Cancel");
        }

        public static void ForceSerialize(string searchKeywords = "")
        {
            var assetPaths = AssetDatabase.FindAssets(searchKeywords)
                .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                .Where(path => !string.IsNullOrEmpty(path))
                .ToArray();

            AssetDatabase.ForceReserializeAssets(assetPaths);
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
            Debug.Log($"Force Reserialized <color=green>{assetPaths.Length}</color> assets.");
        }
    }
}

