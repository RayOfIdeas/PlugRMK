using System;
using UnityEditor;
using UnityEngine;
using static PlugRMK.UnityUti.EditorUti.InspectorHeaderDrawer;

namespace PlugRMK.UnityUti.EditorUti
{
    public static class AudioSourceInspectorHeader
    {
        const string MENU_PLAY = "CONTEXT/AudioSource/Play";

        [MenuItem(MENU_PLAY)]
        static void PlayAudioSource(MenuCommand command)
        {
            var audioSouce = command.context as AudioSource;
            audioSouce.Play();
        }

        [MenuItem(MENU_PLAY, true)]
        static bool PlayAudioSource_Validate(MenuCommand command)
        {
            var audioSouce = command.context as AudioSource;
            return audioSouce != null && audioSouce.clip != null;
        }

        [InspectorHeaderButton]
        public static void DrawHeaderButton(Editor editor)
        {
            if (editor.target is not GameObject gameObject ||
                !gameObject.TryGetComponent(out AudioSource audioSource))
                return;

            GUILayout.BeginHorizontal();
            DrawLabel(EditorIconsName.audiosource_icon);
            if (audioSource.isPlaying)
            {
                if (DrawButton(EditorIconsName.stopbutton))
                    audioSource.Stop();
            }
            else
            {
                if (DrawButton(EditorIconsName.playbutton))
                    audioSource.Play();
            }
            GUILayout.EndHorizontal();
        }
    }
}
