using UnityEditor;
using UnityEngine;

namespace Editor
{
    [InitializeOnLoad]
    public static class CameraBookmarks
    {
        static CameraBookmarks()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }

        static void OnSceneGUI(SceneView view)
        {
            Event e = Event.current;

            // Check for key down events (1 through 9)
            if (e.type == EventType.KeyDown && e.keyCode >= KeyCode.Alpha1 && e.keyCode <= KeyCode.Alpha9)
            {
                int index = e.keyCode - KeyCode.Alpha1 + 1;
                string keyName = $"CamBookmark_{index}";

                // If Ctrl is held -> SAVE
                if (e.control) 
                {
                    var transform = view.camera.transform;
                    string data = $"{transform.position.x}|{transform.position.y}|{transform.position.z}|{transform.rotation.x}|{transform.rotation.y}|{transform.rotation.z}|{transform.rotation.w}|{view.size}";
                
                    EditorPrefs.SetString(keyName, data);
                    Debug.Log($"<color=green>Saved Camera Bookmark {index}</color>");
                    e.Use();
                }
                // If Shift is held -> LOAD
                else if (e.shift) 
                {
                    if (EditorPrefs.HasKey(keyName))
                    {
                        string[] parts = EditorPrefs.GetString(keyName).Split('|');
                        if (parts.Length == 8)
                        {
                            Vector3 pos = new Vector3(float.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2]));
                            Quaternion rot = new Quaternion(float.Parse(parts[3]), float.Parse(parts[4]), float.Parse(parts[5]), float.Parse(parts[6]));
                            float size = float.Parse(parts[7]);

                            view.LookAtDirect(pos, rot, size);
                            Debug.Log($"<color=cyan>Loaded Camera Bookmark {index}</color>");
                        }
                    }
                    e.Use();
                }
            }
        }
    }
}