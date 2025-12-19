using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Editor
{
    public class ProductivityTools : EditorWindow
    {
        [MenuItem("Tools/Productivity Suite")]
        public static void ShowWindow()
        {
            GetWindow<ProductivityTools>("Gravity Tools");
        }

        private string _prefixToAdd = "Pf_";
        private string _replaceFrom = "Old";
        private string _replaceTo = "New";

        private void OnGUI()
        {
            GUILayout.Label("1. BATCH RENAMER (Đổi tên hàng loạt)", EditorStyles.boldLabel);
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("Prefix:", GUILayout.Width(50));
            _prefixToAdd = GUILayout.TextField(_prefixToAdd);
            if (GUILayout.Button("Add Prefix", GUILayout.Width(100)))
            {
                AddPrefixToSelection();
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Replace:", GUILayout.Width(50));
            _replaceFrom = GUILayout.TextField(_replaceFrom, GUILayout.Width(80));
            GUILayout.Label("With:", GUILayout.Width(40));
            _replaceTo = GUILayout.TextField(_replaceTo, GUILayout.Width(80));
            if (GUILayout.Button("Replace", GUILayout.Width(80)))
            {
                ReplaceTextInSelection();
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(20);
            GUILayout.Label("2. LEVEL DESIGN HELPER", EditorStyles.boldLabel);
            if (GUILayout.Button("DROP TO GROUND (Snap xuống đất)", GUILayout.Height(35)))
            {
                DropSelectedToGround();
            }
            GUILayout.Label("Mẹo: Chọn vật thể -> Bấm nút -> Nó tự tìm sàn để đáp xuống.", EditorStyles.miniLabel);

            GUILayout.Space(20);
            GUILayout.Label("3. CLEANUP", EditorStyles.boldLabel);
            if (GUILayout.Button("Remove Missing Scripts (Selected)", GUILayout.Height(30)))
            {
                RemoveMissingScripts();
            }
        }

        // --- LOGIC ĐỔI TÊN ---
        private void AddPrefixToSelection()
        {
            // Lấy các asset đang chọn trong Project
            Object[] selectedAssets = Selection.objects;
            if (selectedAssets.Length == 0) return;

            Undo.RecordObjects(selectedAssets, "Batch Rename");

            foreach (Object obj in selectedAssets)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                // Nếu là file trong Project folder
                if (!string.IsNullOrEmpty(path))
                {
                    string currentName = obj.name;
                    // Tránh duplicate prefix
                    if (currentName.StartsWith(_prefixToAdd)) continue;

                    AssetDatabase.RenameAsset(path, _prefixToAdd + currentName);
                }
            }
            AssetDatabase.SaveAssets();
        }

        private void ReplaceTextInSelection()
        {
            Object[] selectedAssets = Selection.objects;
            foreach (Object obj in selectedAssets)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                if (!string.IsNullOrEmpty(path) && obj.name.Contains(_replaceFrom))
                {
                    string newName = obj.name.Replace(_replaceFrom, _replaceTo);
                    AssetDatabase.RenameAsset(path, newName);
                }
            }
            AssetDatabase.SaveAssets();
        }

        // --- LOGIC LEVEL DESIGN ---
        private void DropSelectedToGround()
        {
            foreach (GameObject go in Selection.gameObjects)
            {
                Undo.RecordObject(go.transform, "Drop To Ground");
                
                // Bắn Raycast xuống dưới
                if (Physics.Raycast(go.transform.position + Vector3.up, Vector3.down, out RaycastHit hit, 100f))
                {
                    // Đặt vật thể tại điểm va chạm
                    // Có thể cộng thêm renderer bounds.extents.y nếu muốn vật nằm trên sàn thay vì tâm chìm xuống sàn
                    go.transform.position = hit.point;
                    
                    // Tùy chọn: Xoay theo độ nghiêng bề mặt (Align to surface normal)
                    // go.transform.up = hit.normal; 
                }
            }
        }

        // --- LOGIC DỌN RÁC ---
        private void RemoveMissingScripts()
        {
            int count = 0;
            foreach (GameObject go in Selection.gameObjects)
            {
                // Cho phép Undo
                Undo.RegisterCompleteObjectUndo(go, "Remove Missing Scripts");
                
                // Hàm này chỉ có từ Unity 2019+
                int removed = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
                count += removed;
            }
            Debug.Log($"<color=green>Đã dọn dẹp {count} script bị lỗi (missing)!</color>");
        }
    }
}