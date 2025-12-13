using UnityEditor;
using UnityEngine;

namespace Editor
{
    [InitializeOnLoad]
    public class ProjectAudioPreview
    {
        static ProjectAudioPreview()
        {
            EditorApplication.projectWindowItemOnGUI += DrawPlayButton;
        }

        static void DrawPlayButton(string guid, Rect selectionRect)
        {
            // Only draw if the rect is wide enough (List view, not Grid view)
            if (selectionRect.width < 50) return;

            // Check if mouse is hovering over this item
            if (!selectionRect.Contains(Event.current.mousePosition)) return;

            // Try to load as AudioClip
            string path = AssetDatabase.GUIDToAssetPath(guid);
            AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);

            if (clip != null)
            {
                // Calculate button position (Right side of the row)
                Rect btnRect = new Rect(selectionRect.x + selectionRect.width - 40, selectionRect.y, 35, selectionRect.height);

                if (GUI.Button(btnRect, "▶"))
                {
                    StopAllClips();
                    PlayClip(clip);
                }
            }
        }

        static void PlayClip(AudioClip clip)
        {
            // We use reflection to access the internal AudioUtil to play without an AudioSource in the scene
            System.Reflection.Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
            System.Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            System.Reflection.MethodInfo method = audioUtilClass.GetMethod(
                "PlayPreviewClip", 
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public, 
                null, 
                new System.Type[] { typeof(AudioClip), typeof(int), typeof(bool) }, 
                null
            );
            method.Invoke(null, new object[] { clip, 0, false });
        }

        static void StopAllClips()
        {
            System.Reflection.Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
            System.Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            System.Reflection.MethodInfo method = audioUtilClass.GetMethod(
                "StopAllPreviewClips", 
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public, 
                null, 
                new System.Type[] { }, 
                null
            );
            method.Invoke(null, new object[] { });
        }
    }
}