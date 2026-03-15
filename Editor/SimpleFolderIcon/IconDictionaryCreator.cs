using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace SimpleFolderIcon.Editor
{
    public class IconDictionaryCreator : AssetPostprocessor
    {
        const string IconsPath = "Packages/com.rayofideas.plugrmk/Editor/SimpleFolderIcon/Icons";
        const string IconsPathWithSlash = "Packages/com.rayofideas.plugrmk/Editor/SimpleFolderIcon/Icons/";
        internal static Dictionary<string, Texture> IconDictionary;

        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            if (!ContainsIconAsset(importedAssets) &&
                !ContainsIconAsset(deletedAssets) &&
                !ContainsIconAsset(movedAssets) &&
                !ContainsIconAsset(movedFromAssetPaths))
            {
                return;
            }

            BuildDictionary();
        }

        static bool ContainsIconAsset(string[] assets)
        {
            foreach (string str in assets)
                if (!string.IsNullOrEmpty(str) && str.StartsWith(IconsPathWithSlash))
                    return true;
            return false;
        }

        internal static void BuildDictionary()
        {
            if (!Directory.Exists(IconsPath))
            {
                Debug.LogWarning($"Directory <color=yellow>{IconsPath}</color> does not exist. Folder icons will not be available.");
                return;
            }

            var dictionary = new Dictionary<string, Texture>();
            var dir = new DirectoryInfo(IconsPath);
            FileInfo[] info = dir.GetFiles("*.png");
            foreach(FileInfo f in info)
            {
                var texture = (Texture)AssetDatabase.LoadAssetAtPath($"{IconsPath}/{f.Name}", typeof(Texture2D));
                dictionary.Add(Path.GetFileNameWithoutExtension(f.Name),texture);
            }
            IconDictionary = dictionary;
        }
    }
}
