using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class PhysicsDropper
    {
        [MenuItem("Tools/Physics/Drop Selected (Simulate) %&d")] // Ctrl + Alt + D
        public static void DropSelected()
        {
            if (Selection.gameObjects.Length == 0) return;

            // Record Undo so you can Ctrl+Z if they fall off the map
            Undo.RecordObjects(Selection.transforms, "Physics Drop");

            // Run simulation for 500 steps (approx 10 seconds of physics)
            // This happens instantly to the user
            int maxIterations = 500;
        
            Physics.autoSimulation = false; // Stop normal physics

            for (int i = 0; i < maxIterations; i++)
            {
                Physics.Simulate(Time.fixedDeltaTime);
            
                // Optional: Break early if objects stop moving (optimization)
                // But usually 500 frames is fast enough to just run fully.
            }

            Physics.autoSimulation = true; // Restore physics
            Debug.Log("Physics Drop Complete.");
        }
    }
}