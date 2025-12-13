using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class SelectionHistory : EditorWindow
    {
        private List<Object> history = new List<Object>();
        private Vector2 scrollPos;

        [MenuItem("Window/General/Selection History")]
        public static void ShowWindow() => GetWindow<SelectionHistory>("History");

        private void OnEnable() => Selection.selectionChanged += OnSelectionChange;
        private void OnDisable() => Selection.selectionChanged -= OnSelectionChange;

        private void OnSelectionChange()
        {
            Object current = Selection.activeObject;
            if (current != null && (history.Count == 0 || history[0] != current))
            {
                history.Insert(0, current);
                if (history.Count > 20) history.RemoveAt(history.Count - 1);
                Repaint();
            }
        }

        private void OnGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        
            if (GUILayout.Button("Clear History")) history.Clear();
            GUILayout.Space(10);

            for (int i = 0; i < history.Count; i++)
            {
                if (history[i] == null) continue; // Skip deleted objects

                EditorGUILayout.BeginHorizontal();
            
                // Ping button (highlight in project/hierarchy)
                if (GUILayout.Button("?", GUILayout.Width(25))) 
                    EditorGUIUtility.PingObject(history[i]);

                // Select button
                if (GUILayout.Button(history[i].name, EditorStyles.label)) 
                    Selection.activeObject = history[i];

                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
        }
    }
}