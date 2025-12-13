using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class TransformTools
    {
        [MenuItem("GameObject/Tools/Snap to Ground %g")] // Shortcut: Ctrl/Cmd + G
        public static void SnapToGround()
        {
            foreach (var transform in Selection.transforms)
            {
                Undo.RecordObject(transform, "Snap to Ground");
            
                RaycastHit hit;
                // Raycast down from object position
                if (Physics.Raycast(transform.position, Vector3.down, out hit))
                {
                    transform.position = hit.point;
                }
                else
                {
                    Debug.LogWarning($"Could not find ground beneath {transform.name}");
                }
            }
        }

        [MenuItem("GameObject/Tools/Group Selected %e")] // Shortcut: Ctrl/Cmd + E
        public static void GroupSelected()
        {
            if (Selection.transforms.Length == 0) return;

            GameObject newParent = new GameObject("Group");
            Undo.RegisterCreatedObjectUndo(newParent, "Group Selected");

            // Position the new parent at the center of selection
            Vector3 center = Vector3.zero;
            foreach (var t in Selection.transforms) center += t.position;
            center /= Selection.transforms.Length;
            newParent.transform.position = center;

            foreach (var t in Selection.transforms)
            {
                Undo.SetTransformParent(t, newParent.transform, "Group Selected");
            }
        
            Selection.activeGameObject = newParent;
        }
    }
}