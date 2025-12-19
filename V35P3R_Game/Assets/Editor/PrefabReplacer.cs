using UnityEngine;
using UnityEditor;

namespace Editor
{
    public class PrefabReplacer : EditorWindow
    {
        [MenuItem("Tools/Prefab Replacer (Greybox to Art)")]
        public static void ShowWindow()
        {
            GetWindow<PrefabReplacer>("Replacer");
        }

        private GameObject _newPrefab;
        private bool _keepRotation = true;
        private bool _keepScale = true;
        private bool _keepNames = false;

        private void OnGUI()
        {
            GUILayout.Label("THAY THẾ OBJECT HÀNG LOẠT", EditorStyles.boldLabel);
            GUILayout.Space(10);

            _newPrefab = (GameObject)EditorGUILayout.ObjectField("Prefab Mới", _newPrefab, typeof(GameObject), false);

            GUILayout.Space(10);
            _keepRotation = EditorGUILayout.Toggle("Giữ Rotation cũ", _keepRotation);
            _keepScale = EditorGUILayout.Toggle("Giữ Scale cũ", _keepScale);
            _keepNames = EditorGUILayout.Toggle("Giữ Tên cũ", _keepNames);

            GUILayout.Space(20);
            GUI.enabled = _newPrefab != null && Selection.gameObjects.Length > 0;
            if (GUILayout.Button("REPLACE SELECTED", GUILayout.Height(40)))
            {
                Replace();
            }
            GUI.enabled = true;

            GUILayout.Label($"Đang chọn: {Selection.gameObjects.Length} objects", EditorStyles.helpBox);
        }

        private void Replace()
        {
            GameObject[] selection = Selection.gameObjects;
            
            // Group Undo để có thể Ctrl+Z 1 phát là hoàn tác tất cả
            Undo.SetCurrentGroupName("Replace Prefabs");
            int group = Undo.GetCurrentGroup();

            foreach (GameObject oldObj in selection)
            {
                // Instantiate as Prefab connection
                GameObject newObj = (GameObject)PrefabUtility.InstantiatePrefab(_newPrefab);
                
                // Đăng ký Undo cho việc tạo object mới
                Undo.RegisterCreatedObjectUndo(newObj, "Create New");

                // Copy Transform
                newObj.transform.position = oldObj.transform.position;
                newObj.transform.parent = oldObj.transform.parent;
                newObj.transform.SetSiblingIndex(oldObj.transform.GetSiblingIndex());

                if (_keepRotation) newObj.transform.rotation = oldObj.transform.rotation;
                if (_keepScale) newObj.transform.localScale = oldObj.transform.localScale;
                if (_keepNames) newObj.name = oldObj.name;

                // Xóa object cũ
                Undo.DestroyObjectImmediate(oldObj);
            }

            Undo.CollapseUndoOperations(group);
            Selection.objects = new Object[0]; // Bỏ chọn để tránh lỗi
            Debug.Log($"<color=green>Đã thay thế {selection.Length} object thành công!</color>");
        }
    }
}