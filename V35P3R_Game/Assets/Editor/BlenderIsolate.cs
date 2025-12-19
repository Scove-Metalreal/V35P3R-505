using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class BlenderIsolate
    {
        // Toggle Isolate View
        [MenuItem("Tools/View/Toggle Isolate Selection #i")] // Shift + I
        static void ToggleIsolate()
        {
            if (SceneVisibilityManager.instance.IsCurrentStageIsolated())
            {
                SceneVisibilityManager.instance.ExitIsolation();
            }
            else
            {
                if (Selection.gameObjects.Length > 0)
                {
                    SceneVisibilityManager.instance.Isolate(Selection.gameObjects, true);
                }
                else
                {
                    Debug.LogWarning("Select an object to isolate.");
                }
            }
        }
    }
}