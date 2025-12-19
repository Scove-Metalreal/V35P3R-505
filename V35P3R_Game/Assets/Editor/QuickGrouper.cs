using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace Editor
{
    public class QuickGrouper
    {
        [MenuItem("GameObject/Group Selected %g", false, 0)] // %g nghĩa là Ctrl + G
        private static void GroupSelected()
        {
            if (Selection.transforms.Length == 0) return;

            // 1. Tạo object cha mới
            GameObject groupParent = new GameObject("New_Group");
            Undo.RegisterCreatedObjectUndo(groupParent, "Group Selected");

            // 2. Tính toán tâm của các object con
            Vector3 center = Vector3.zero;
            foreach (Transform t in Selection.transforms)
            {
                center += t.position;
            }
            center /= Selection.transforms.Length;
            groupParent.transform.position = center;

            // 3. Gom con vào cha (vẫn giữ nguyên vị trí thế giới)
            // Sắp xếp theo index để giữ thứ tự hierarchy
            var sortedTransforms = new List<Transform>(Selection.transforms);
            sortedTransforms.Sort((a, b) => a.GetSiblingIndex().CompareTo(b.GetSiblingIndex()));

            // Nếu các con cùng 1 cha cũ, thì cha mới cũng nằm trong cha cũ đó
            Transform commonParent = sortedTransforms[0].parent;
            if (commonParent != null)
            {
                groupParent.transform.SetParent(commonParent);
                groupParent.transform.SetSiblingIndex(sortedTransforms[0].GetSiblingIndex());
            }

            foreach (Transform t in sortedTransforms)
            {
                Undo.SetTransformParent(t, groupParent.transform, "Group Selected");
            }

            // 4. Chọn cha mới
            Selection.activeGameObject = groupParent;
        }
    }
}