using UnityEditor;
using UnityEngine;

namespace Editor
{
    [InitializeOnLoad]
    public class HierarchySeparators
    {
        static HierarchySeparators()
        {
            EditorApplication.hierarchyWindowItemOnGUI += HandleHierarchyWindowItemOnGUI;
        }

        private static void HandleHierarchyWindowItemOnGUI(int selectionID, Rect selectionRect)
        {
            GameObject go = EditorUtility.InstanceIDToObject(selectionID) as GameObject;
            if (go != null && go.name.StartsWith("//"))
            {
                // Calculate Color (Dark Grey)
                EditorGUI.DrawRect(selectionRect, new Color(0.2f, 0.2f, 0.2f));

                // Create Style
                GUIStyle style = new GUIStyle();
                style.normal.textColor = Color.white;
                style.fontStyle = FontStyle.Bold;
                style.alignment = TextAnchor.MiddleCenter;

                // Draw Text (Remove the // for display)
                EditorGUI.LabelField(selectionRect, go.name.Replace("//", "").ToUpper(), style);
            
                // Prevent selecting the separator if you want it to be purely visual (Optional)
                // if (Event.current.type == EventType.MouseDown && selectionRect.Contains(Event.current.mousePosition))
                //     Event.current.Use();
            }
        }
    }
}