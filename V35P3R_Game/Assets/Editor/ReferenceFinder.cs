using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class ReferenceFinder
    {
        [MenuItem("Assets/Find References In Project", false, 20)]
        private static void FindRefs()
        {
            Object selected = Selection.activeObject;
            if (!selected) return;

            string path = AssetDatabase.GetAssetPath(selected);
            string guid = AssetDatabase.AssetPathToGUID(path);

            // Find all assets that contain this GUID
            string[] allGuids = AssetDatabase.FindAssets("t:Prefab t:Scene");
        
            List<string> foundIn = new List<string>();
            int count = 0;

            EditorUtility.DisplayProgressBar("Searching", "Scanning assets...", 0f);

            for (int i = 0; i < allGuids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(allGuids[i]);
            
                // Progress bar update every 100 items
                if (i % 100 == 0) 
                    EditorUtility.DisplayProgressBar("Searching", $"Scanning {i}/{allGuids.Length}", (float)i/allGuids.Length);

                // Read the file as text to check for GUID dependency
                string content = System.IO.File.ReadAllText(assetPath);
                if (content.Contains(guid))
                {
                    foundIn.Add(assetPath);
                    count++;
                }
            }

            EditorUtility.ClearProgressBar();

            Debug.Log($"<color=cyan>Found {count} references for {selected.name}:</color>");
            foreach (string p in foundIn)
            {
                Debug.Log($"- {p}");
            }
        }
    }
}