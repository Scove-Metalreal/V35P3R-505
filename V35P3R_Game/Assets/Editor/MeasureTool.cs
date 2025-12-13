using UnityEditor;
using UnityEngine;

namespace Editor
{
    [InitializeOnLoad]
    public class MeasureTool
    {
        static MeasureTool()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }

        static void OnSceneGUI(SceneView view)
        {
            if (Selection.transforms.Length != 2) return;

            Transform t1 = Selection.transforms[0];
            Transform t2 = Selection.transforms[1];

            if (t1 == null || t2 == null) return;

            Vector3 p1 = t1.position;
            Vector3 p2 = t2.position;
            float distance = Vector3.Distance(p1, p2);
            Vector3 midPoint = (p1 + p2) * 0.5f;

            // Draw Line
            Handles.color = Color.yellow;
            Handles.DrawDottedLine(p1, p2, 4f);

            // Draw Label
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.yellow;
            style.fontSize = 15;
            style.fontStyle = FontStyle.Bold;
        
            Handles.Label(midPoint + Vector3.up * 0.2f, $"{distance:F2}m", style);
        }
    }
}