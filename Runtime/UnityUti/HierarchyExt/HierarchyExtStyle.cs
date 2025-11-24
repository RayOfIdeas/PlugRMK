using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlugRMK.UnityUti.Hext
{
    [System.Serializable]
    public class HierarchyExtStyle
    {
        public string token;
        public bool removeTokenFromLabel = false;
        public string labelName;

        [Header("Text Style")]
        public Color textColor = new(0.9f, 0.9f, 0.9f);
        public TextAnchor textAlignment = TextAnchor.UpperLeft;
        public FontStyle fontStyle = FontStyle.Normal;
        public Vector2Int textOffset = new (18, 0);

        [Header("BG Style")]
        public bool isCustomBG = false;
        public Color bgColor = new(0.2196f, 0.2196f, 0.2196f); // UnityEditorColorUtility.k_defaultProColor;
        public Color bgColorHover = new(0.2706f, 0.2706f, 0.2706f); // UnityEditorColorUtility.k_hoveredProColor;
        public Color bgColorSelect = new(0.1725f, 0.3647f, 0.5294f); // UnityEditorColorUtility.k_selectedProColor;
        public bool isCustomBGOffset = false;
        public Vector2 bgOffset = new (0, 0);
        public static readonly Vector2 bgOffsetDefault = new(16, 0);

        [Header("Icon")]
        public Sprite icon;

        public Color GetBGColor(bool isSelected, bool isHovered, bool isWindowFocused)
        {
            if (isSelected)
            {
                if (isWindowFocused)
                {
                    return bgColorSelect;
                }
                else
                {
                    return bgColorHover;
                }
            }
            else if (isHovered)
            {
                return bgColorHover;
            }
            else
            {
                return bgColor;
            }
        }
    }

}
