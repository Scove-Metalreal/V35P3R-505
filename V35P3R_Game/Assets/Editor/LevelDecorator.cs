using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class LevelDecorator : EditorWindow
    {
        [MenuItem("Tools/Level Decorator (Chaos Maker)")]
        public static void ShowWindow()
        {
            GetWindow<LevelDecorator>("Level Decorator");
        }

        private Vector3 minScale = new Vector3(0.9f, 0.9f, 0.9f);
        private Vector3 maxScale = new Vector3(1.1f, 1.1f, 1.1f);
        private float maxRotationY = 180f;
        private float maxTilt = 5f;

        private void OnGUI()
        {
            GUILayout.Label("RANDOMIZE TRANSFORM", EditorStyles.boldLabel);
            
            GUILayout.Space(10);
            GUILayout.Label("1. Rotation (Xoay)", EditorStyles.label);
            maxRotationY = EditorGUILayout.Slider("Random Y Axis (0-360)", maxRotationY, 0, 360);
            maxTilt = EditorGUILayout.Slider("Random Tilt (Nghiêng)", maxTilt, 0, 15);

            if (GUILayout.Button("Apply Random Rotation", GUILayout.Height(30)))
            {
                ApplyRotation();
            }

            GUILayout.Space(20);
            GUILayout.Label("2. Scale (Kích thước)", EditorStyles.label);
            minScale = EditorGUILayout.Vector3Field("Min Scale", minScale);
            maxScale = EditorGUILayout.Vector3Field("Max Scale", maxScale);

            if (GUILayout.Button("Apply Random Scale", GUILayout.Height(30)))
            {
                ApplyScale();
            }
            
            GUILayout.Space(10);
            EditorGUILayout.HelpBox("Chọn các object trong Scene trước khi bấm nút.", MessageType.Info);
        }

        private void ApplyRotation()
        {
            foreach (Transform t in Selection.transforms)
            {
                Undo.RecordObject(t, "Random Rotation");
                Vector3 currentRot = t.localEulerAngles;
                
                // Xoay quanh trục Y ngẫu nhiên
                float randY = Random.Range(-maxRotationY, maxRotationY);
                
                // Nghiêng nhẹ (cho giống đồ vật cũ kỹ)
                float randX = Random.Range(-maxTilt, maxTilt);
                float randZ = Random.Range(-maxTilt, maxTilt);

                t.localEulerAngles = new Vector3(currentRot.x + randX, currentRot.y + randY, currentRot.z + randZ);
            }
        }

        private void ApplyScale()
        {
            foreach (Transform t in Selection.transforms)
            {
                Undo.RecordObject(t, "Random Scale");
                
                float rX = Random.Range(minScale.x, maxScale.x);
                float rY = Random.Range(minScale.y, maxScale.y);
                float rZ = Random.Range(minScale.z, maxScale.z);

                // Giữ tỉ lệ đều (Uniform) nếu muốn
                // t.localScale = Vector3.one * rX; 
                
                t.localScale = new Vector3(rX, rY, rZ);
            }
        }
    }
}