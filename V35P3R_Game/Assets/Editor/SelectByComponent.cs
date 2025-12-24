using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class SelectByComponent : EditorWindow
    {
        MonoScript targetScript;
        Component targetComponent; // For built-in types

        [MenuItem("Tools/Selection/Select By Component")]
        static void Init() => GetWindow<SelectByComponent>("Select By Type");

        void OnGUI()
        {
            GUILayout.Label("Select Objects with Component:", EditorStyles.boldLabel);
        
            // Slot for a script file
            targetScript = (MonoScript)EditorGUILayout.ObjectField("Script File", targetScript, typeof(MonoScript), false);

            if (GUILayout.Button("Select All Occurrences"))
            {
                if (targetScript == null) return;
            
                System.Type type = targetScript.GetClass();
                if (type == null) return;

                // Find all objects
                var foundComponents = FindObjectsByType(type, FindObjectsSortMode.None) as Component[];
                GameObject[] gameObjects = foundComponents.Select(c => c.gameObject).ToArray();

                Selection.objects = gameObjects;
                Debug.Log($"Selected {gameObjects.Length} objects with component {type.Name}");
            }
        }
    }
}