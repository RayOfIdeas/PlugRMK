using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace PlugRMK.UnityUti.EditorUti
{
    public static class CreateScriptTemplate
    {
        const string TEMPLATES = "ScriptTemplates";
        const int PRIORITY_INDEX = -100;

        [MenuItem("Assets/Create/Scripting/MonoBehaviour Custom", priority = PRIORITY_INDEX, secondaryPriority = 0)]
        public static void CreateMonoBehaviour() => CreateScriptFromTemplateName("MonoBehaviour");

        [MenuItem("Assets/Create/Scripting/ScriptableObject Custom", priority = PRIORITY_INDEX, secondaryPriority = 1)]
        public static void CreateScriptableObject() => CreateScriptFromTemplateName("ScriptableObject");

        [MenuItem("Assets/Create/Scripting/Static Class", priority = PRIORITY_INDEX, secondaryPriority = 2)]
        public static void CreateStaticClass() => CreateScriptFromTemplateName("StaticClass");

        [MenuItem("Assets/Create/Scripting/Interface", priority = PRIORITY_INDEX, secondaryPriority = 3)]
        public static void CreateInterface() => CreateScriptFromTemplateName("Interface");

        [MenuItem("Assets/Create/Scripting/UGUI Editor Window", priority = PRIORITY_INDEX, secondaryPriority = 4)]
        public static void CreateUGUIEditorWindow() => CreateScriptFromTemplateName("UGUIEditorWindow");

        [MenuItem("Assets/Create/Scripting/UI Toolkit Editor Window", priority = PRIORITY_INDEX, secondaryPriority = 5)]
        public static void CreateUIToolkitEditorWindow() => CreateScriptFromTemplateName("UIToolkit_EditorWindow");

        [MenuItem("Assets/Create/Scripting/UI Toolkit Inspector", priority = PRIORITY_INDEX, secondaryPriority = 6)]
        public static void CreateUIToolkitInspector() => CreateScriptFromTemplateName("UIToolkit_Inspector");

        [MenuItem("Assets/Create/Scripting/UI Toolkit Property Drawer", priority = PRIORITY_INDEX, secondaryPriority = 7)]
        public static void CreateUIToolkitPropertyDrawer() => CreateScriptFromTemplateName("UIToolkit_PropertyDrawer");

        [MenuItem("Assets/Create/Scripting/Editor Preference", priority = PRIORITY_INDEX, secondaryPriority = 8)]
        public static void CreateEditorPreference() => CreateScriptFromTemplateName("EditorPreference");

        public static void CreateScriptFromTemplateName(string templateName)
        {
            var parentPath = GetParentPath(nameof(CreateScriptTemplate), templateName);
            var templatePath = $"{parentPath}/{TEMPLATES}/{templateName}.cs.txt";
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templatePath, $"New{templateName}.cs");
        }

        public static string GetParentPath(string assetName, string childFileName)
        {
            var guids = AssetDatabase.FindAssets(assetName);
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var folderPath = path[..(path.Length - "/".Length - assetName.Length - ".cs".Length)];
                var templatePath = $"{folderPath}/{TEMPLATES}/{childFileName}.cs.txt";
                if (File.Exists(templatePath))
                    return folderPath;
            }

            Debug.LogError($"Cannot find parent path of {childFileName}\nGUID count: {guids.Length}");
            return "";
        }
    }
}
