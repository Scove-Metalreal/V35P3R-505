using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

namespace Editor
{
    public class SceneTeleporter : EditorWindow
    {
        [MenuItem("Tools/Scene Teleporter (Copy-Paste Cross Scene)")]
        public static void ShowWindow()
        {
            GetWindow<SceneTeleporter>("Teleporter");
        }

        private const string TEMP_PATH = "Assets/_Gravity_Temp_Clipboard.prefab";
        private bool keepWorldPosition = true;

        private void OnGUI()
        {
            GUILayout.Label("CHUYỂN ĐỒ VẬT GIỮA CÁC SCENE", EditorStyles.boldLabel);
            GUILayout.Space(10);

            // --- COPY SECTION ---
            GUILayout.BeginVertical("box");
            GUILayout.Label("1. Tại Scene Gốc (Source):", EditorStyles.miniLabel);
            if (GUILayout.Button("COPY SELECTED OBJECTS", GUILayout.Height(40)))
            {
                CopyObjects();
            }
            GUILayout.EndVertical();

            GUILayout.Space(10);
            
            // --- PASTE SECTION ---
            GUILayout.BeginVertical("box");
            GUILayout.Label("2. Tại Scene Đích (Target):", EditorStyles.miniLabel);
            
            keepWorldPosition = EditorGUILayout.Toggle("Giữ nguyên tọa độ cũ", keepWorldPosition);

            GUI.enabled = File.Exists(TEMP_PATH); // Chỉ hiện nút Paste nếu đã có file copy
            if (GUILayout.Button("PASTE FROM CLIPBOARD", GUILayout.Height(40)))
            {
                PasteObjects();
            }
            GUI.enabled = true;
            
            if (File.Exists(TEMP_PATH))
            {
                EditorGUILayout.HelpBox("Đang có dữ liệu trong Clipboard!", MessageType.Info);
                if (GUILayout.Button("Xóa Clipboard"))
                {
                    AssetDatabase.DeleteAsset(TEMP_PATH);
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Clipboard trống.", MessageType.None);
            }
            GUILayout.EndVertical();
        }

        private void CopyObjects()
        {
            GameObject[] selection = Selection.gameObjects;
            if (selection.Length == 0)
            {
                Debug.LogWarning("Chưa chọn object nào để copy!");
                return;
            }

            // 1. Tạo một container tạm để chứa tất cả object đã chọn
            GameObject container = new GameObject("Temp_Container");
            
            // 2. Duplicate các object chọn vào container (để không ảnh hưởng object gốc)
            foreach (GameObject go in selection)
            {
                GameObject clone = Instantiate(go, container.transform);
                clone.name = go.name; // Giữ tên cũ
                
                // Giữ nguyên transform relative với world cũ
                clone.transform.position = go.transform.position;
                clone.transform.rotation = go.transform.rotation;
                clone.transform.localScale = go.transform.localScale;
            }

            // 3. Lưu container thành Prefab tạm thời
            PrefabUtility.SaveAsPrefabAsset(container, TEMP_PATH);

            // 4. Dọn dẹp scene hiện tại (Xóa container tạm đi)
            DestroyImmediate(container);

            Debug.Log($"<color=green>[Teleporter]</color> Đã copy {selection.Length} object vào bộ nhớ đệm!");
        }

        private void PasteObjects()
        {
            // 1. Load Prefab từ bộ nhớ đệm
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(TEMP_PATH);
            if (prefab == null) return;

            // 2. Instantiate vào scene mới
            GameObject container = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            
            // 3. Bung Prefab (Unpack) để nó trở thành object thường
            PrefabUtility.UnpackPrefabInstance(container, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

            // 4. Đưa các con ra ngoài và xóa container
            List<GameObject> pastedObjects = new List<GameObject>();
            
            // Dùng vòng lặp ngược để tách con ra an toàn
            while (container.transform.childCount > 0)
            {
                Transform child = container.transform.GetChild(0);
                child.SetParent(null); // Đưa ra root hoặc cha mới
                
                // Nếu không giữ vị trí cũ thì đưa về 0,0,0 hoặc trước camera
                if (!keepWorldPosition)
                {
                    child.position = SceneView.lastActiveSceneView.camera.transform.position + Vector3.forward * 2f;
                }
                
                pastedObjects.Add(child.gameObject);
            }

            // 5. Xóa vỏ container và file tạm (Tuỳ chọn: Giữ file tạm để paste nhiều lần)
            DestroyImmediate(container);
            // AssetDatabase.DeleteAsset(TEMP_PATH); // Bỏ comment nếu muốn paste xong là xóa luôn

            // 6. Select các object vừa paste để tiện thao tác
            Selection.objects = pastedObjects.ToArray();

            Debug.Log($"<color=green>[Teleporter]</color> Đã paste xong! (Clipboard vẫn giữ nguyên để paste tiếp)");
        }
    }
}