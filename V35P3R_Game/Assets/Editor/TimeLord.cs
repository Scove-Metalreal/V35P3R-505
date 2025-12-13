using UnityEditor;
using UnityEngine;

namespace Editor
{
    [InitializeOnLoad]
    public class TimeLord
    {
        static TimeLord()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }

        private static void OnSceneGUI(SceneView sceneView)
        {
            // Chỉ hiện khi đang Play game
            if (!Application.isPlaying) return;

            Handles.BeginGUI();

            // Vẽ hộp công cụ ở góc trên giữa màn hình Scene
            float width = 320f;
            float height = 40f;
            float x = (sceneView.position.width - width) / 2;
            float y = 10f;

            GUILayout.BeginArea(new Rect(x, y, width, height), "TIME LORD", GUI.skin.window);
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Pause", GUILayout.Height(20)))
            {
                EditorApplication.isPaused = !EditorApplication.isPaused;
            }

            // Các mức tốc độ
            if (GUILayout.Button("0.1x", GUILayout.Height(20))) Time.timeScale = 0.1f;
            if (GUILayout.Button("0.5x", GUILayout.Height(20))) Time.timeScale = 0.5f;
            if (GUILayout.Button("1x", GUILayout.Height(20))) Time.timeScale = 1f;
            if (GUILayout.Button("2x", GUILayout.Height(20))) Time.timeScale = 2f;
            if (GUILayout.Button("5x", GUILayout.Height(20))) Time.timeScale = 5f;

            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            Handles.EndGUI();
        }
    }
}