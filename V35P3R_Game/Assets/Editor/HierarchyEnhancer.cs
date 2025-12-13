using UnityEditor;
using UnityEngine;

namespace Editor
{
    [InitializeOnLoad]
    public class HierarchyEnhancer
    {
        static HierarchyEnhancer()
        {
            EditorApplication.hierarchyWindowItemOnGUI += HandleHierarchyWindowItemOnGUI;
        }

        private static void HandleHierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
        {
            GameObject obj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (obj == null) return;

            // Tính toán vị trí vẽ nút Toggle (Bên phải cùng)
            Rect toggleRect = new Rect(selectionRect);
            toggleRect.x = selectionRect.width + selectionRect.x - 16f; // Sát lề phải
            toggleRect.width = 16f;

            // Logic vẽ Toggle
            bool isActive = obj.activeSelf;
            
            // Xử lý Undo để không bị lỗi khi Ctrl+Z
            EditorGUI.BeginChangeCheck();
            bool newActive = EditorGUI.Toggle(toggleRect, isActive);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(obj, "Toggle Active State");
                obj.SetActive(newActive);
            }
        }
    }
}