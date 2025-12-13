using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class BatchRename : EditorWindow
    {
        string baseName = "Object_";
        int startNumber = 1;

        [MenuItem("Tools/Batch Rename")]
        public static void ShowWindow()
        {
            GetWindow<BatchRename>("Batch Rename");
        }

        void OnGUI()
        {
            GUILayout.Label("Batch Rename Selected Objects", EditorStyles.boldLabel);
            baseName = EditorGUILayout.TextField("Base Name", baseName);
            startNumber = EditorGUILayout.IntField("Start Number", startNumber);

            if (GUILayout.Button("Rename"))
            {
                GameObject[] selected = Selection.gameObjects;
            
                // Sort by hierarchy index to keep order
                System.Array.Sort(selected, (a, b) => a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex()));

                for (int i = 0; i < selected.Length; i++)
                {
                    Undo.RecordObject(selected[i], "Batch Rename");
                    selected[i].name = $"{baseName}{(startNumber + i)}";
                }
            }
        }
    }
}