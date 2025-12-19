using UnityEngine;
using UnityEditor;

namespace Editor
{
    public class PolyCounter : EditorWindow
    {
        [MenuItem("Tools/Simple Poly Counter")]
        public static void ShowWindow()
        {
            GetWindow<PolyCounter>("Poly Count");
        }

        private int _totalVerts;
        private int _totalTris;
        private int _meshCount;

        private void OnSelectionChange()
        {
            CountPolys();
            Repaint();
        }

        private void OnGUI()
        {
            GUILayout.Label("SELECTION STATS (Chỉ số vùng chọn)", EditorStyles.boldLabel);
            GUILayout.Space(10);

            EditorGUILayout.LabelField("Objects Selected:", Selection.gameObjects.Length.ToString());
            EditorGUILayout.LabelField("Meshes Found:", _meshCount.ToString());
            
            GUILayout.Space(10);
            // Dùng style to rõ
            GUIStyle bigStyle = new GUIStyle(EditorStyles.boldLabel);
            bigStyle.fontSize = 14;

            GUILayout.Label($"Triangles: {_totalTris:N0}", bigStyle);
            GUILayout.Label($"Vertices:  {_totalVerts:N0}", bigStyle);

            if (_totalTris > 2000) 
            {
                EditorGUILayout.HelpBox("Cảnh báo: Object này hơi nặng cho game Low-poly!", MessageType.Warning);
            }
        }

        private void CountPolys()
        {
            _totalVerts = 0;
            _totalTris = 0;
            _meshCount = 0;

            foreach (GameObject go in Selection.gameObjects)
            {
                // Lấy tất cả MeshFilter trong object và con cái nó
                MeshFilter[] meshes = go.GetComponentsInChildren<MeshFilter>();
                
                foreach (MeshFilter mf in meshes)
                {
                    if (mf.sharedMesh != null)
                    {
                        _totalVerts += mf.sharedMesh.vertexCount;
                        _totalTris += mf.sharedMesh.triangles.Length / 3;
                        _meshCount++;
                    }
                }

                // Lấy cả SkinnedMeshRenderer (cho nhân vật)
                SkinnedMeshRenderer[] skins = go.GetComponentsInChildren<SkinnedMeshRenderer>();
                foreach (SkinnedMeshRenderer smr in skins)
                {
                    if (smr.sharedMesh != null)
                    {
                        _totalVerts += smr.sharedMesh.vertexCount;
                        _totalTris += smr.sharedMesh.triangles.Length / 3;
                        _meshCount++;
                    }
                }
            }
        }
    }
}