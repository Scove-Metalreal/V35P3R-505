using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class UIAnchorTool : MonoBehaviour
    {
        [MenuItem("Tools/UI/Snap Anchors to Corners %&a")] // Alt + Shift + A
        static void SnapAnchors()
        {
            foreach (GameObject go in Selection.gameObjects)
            {
                RectTransform t = go.GetComponent<RectTransform>();
                if (t == null || t.parent == null) continue;

                Undo.RecordObject(t, "Snap Anchors");

                RectTransform parent = t.parent as RectTransform;
                if (parent == null) continue;

                Rect parentRect = parent.rect;
            
                // Calculate new anchors based on current position/size relative to parent
                Vector2 newAnchorMin = new Vector2(
                    t.anchorMin.x + t.offsetMin.x / parentRect.width,
                    t.anchorMin.y + t.offsetMin.y / parentRect.height
                );
                Vector2 newAnchorMax = new Vector2(
                    t.anchorMax.x + t.offsetMax.x / parentRect.width,
                    t.anchorMax.y + t.offsetMax.y / parentRect.height
                );

                // Apply
                t.anchorMin = newAnchorMin;
                t.anchorMax = newAnchorMax;
            
                // Reset offsets to zero so size/pos doesn't change visually
                t.offsetMin = Vector2.zero;
                t.offsetMax = Vector2.zero;
            }
        }
    }
}