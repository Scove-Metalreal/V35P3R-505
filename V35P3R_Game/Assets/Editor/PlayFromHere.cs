using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class PlayFromHere
    {
        [MenuItem("Tools/Move Player to Camera View ^&p")] // Ctrl + Alt + P
        public static void TeleportPlayer()
        {
            // Find object with Player tag
            GameObject player = GameObject.FindGameObjectWithTag("Player");
        
            if (player == null)
            {
                Debug.LogError("No object found with tag 'Player' in the scene.");
                return;
            }

            // Get Scene View Camera
            Camera sceneCam = SceneView.lastActiveSceneView.camera;
            if (sceneCam == null) return;

            Undo.RecordObject(player.transform, "Teleport Player");

            // Move Player
            player.transform.position = sceneCam.transform.position;
        
            // Optional: Rotate player to face view direction (keep Y rotation only for FPS/TPS)
            Vector3 rot = sceneCam.transform.rotation.eulerAngles;
            player.transform.rotation = Quaternion.Euler(0, rot.y, 0);

            Debug.Log("Player teleported to Scene View.");
        
            // Optional: Auto-start game? Uncomment below:
            // EditorApplication.isPlaying = true;
        }
    }
}