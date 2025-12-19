using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class PivotTools
    {
        [MenuItem("Tools/Center Pivot on Children &c")] // Alt + C
        public static void CenterPivot()
        {
            Transform parent = Selection.activeTransform;
            if (parent == null || parent.childCount == 0) return;

            Undo.RecordObject(parent, "Center Pivot");
        
            // 1. Calculate Center
            Vector3 center = Vector3.zero;
            foreach (Transform child in parent)
            {
                center += child.position;
            }
            center /= parent.childCount;

            // 2. Record children positions before moving parent
            Transform[] children = new Transform[parent.childCount];
            Vector3[] originalPos = new Vector3[parent.childCount];
        
            for (int i = 0; i < parent.childCount; i++)
            {
                children[i] = parent.GetChild(i);
                originalPos[i] = children[i].position;
                Undo.RecordObject(children[i], "Center Pivot");
            }

            // 3. Move Parent
            parent.position = center;

            // 4. Restore Children Positions (because moving parent moved them)
            for (int i = 0; i < children.Length; i++)
            {
                children[i].position = originalPos[i];
            }
        
            Debug.Log($"Centered Pivot for {parent.name}");
        }
    }
}