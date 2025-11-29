using System;
using UnityEditor;
using UnityEngine;

namespace PlugRMK.UnityUti.EditorUti
{
    public static class AudioSourceHeaderGUI
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

        [InitializeOnLoadMethod]
        static void HeaderButton()
        {
            Editor.finishedDefaultHeaderGUI += OnFinishedHeaderGUI;
        }

        static void OnFinishedHeaderGUI(Editor editor)
        {
            if (editor.target is not GameObject gameObject ||
                !gameObject.TryGetComponent(out AudioSource audioSource))
                return;

            GUILayout.BeginHorizontal();
            GUILayout.Space(40);

            GUILayout.Label(EditorGUIUtility.IconContent(EditorIconsName.audiosource_icon), GUILayout.Width(20), GUILayout.Height(EditorGUIUtility.singleLineHeight));
            if (audioSource.isPlaying)
            {
                if (GUILayout.Button(EditorGUIUtility.IconContent(EditorIconsName.stopbutton), GUILayout.Width(64)))
                    audioSource.Stop();
            }
            else
            {
                if (GUILayout.Button(EditorGUIUtility.IconContent(EditorIconsName.playbutton), GUILayout.Width(64)))
                    audioSource.Play();
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
    }
}
