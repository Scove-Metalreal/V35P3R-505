using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using System.IO;

namespace Editor
{
    public class SceneDashboard : EditorWindow
    {
        [MenuItem("Tools/Gravity Dashboard")]
        public static void ShowWindow()
        {
            GetWindow<SceneDashboard>("Gravity Dashboard");
        }

        private void OnGUI()
        {
            GUILayout.Label("SCENE NAVIGATION", EditorStyles.boldLabel);
            
            // Nút chuyển Scene nhanh
            if (GUILayout.Button("Load BOOT (00)", GUILayout.Height(30))) OpenScene("Sc_00_Boot");
            if (GUILayout.Button("Load MENU (01)", GUILayout.Height(30))) OpenScene("Sc_01_Menu");
            if (GUILayout.Button("Load GAME (02)", GUILayout.Height(30))) OpenScene("Sc_02_Game");

            GUILayout.Space(20);
            GUILayout.Label("DATA MANAGEMENT", EditorStyles.boldLabel);

            // Nút xóa dữ liệu test (Cực quan trọng khi làm tính năng Save Game)
            if (GUILayout.Button("Clear PlayerPrefs & Save Data", GUILayout.Height(40)))
            {
                if (EditorUtility.DisplayDialog("Clear Data", 
                    "Bạn có chắc muốn xóa sạch dữ liệu Save và PlayerPrefs không?", "Xóa Ngay", "Thôi"))
                {
                    ClearAllData();
                }
            }
        }

        private void OpenScene(string sceneName)
        {
            // Tìm scene trong thư mục _Project
            string[] guids = AssetDatabase.FindAssets($"{sceneName} t:Scene", new[] { "Assets/_Project/Scenes" });
            
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    EditorSceneManager.OpenScene(path);
                }
            }
            else
            {
                Debug.LogError($"Không tìm thấy scene tên: {sceneName}. Hãy chắc chắn bạn đã tạo nó bằng Setup Tool.");
            }
        }

        private void ClearAllData()
        {
            PlayerPrefs.DeleteAll();
            string savePath = Path.Combine(Application.persistentDataPath, "save_data.json");
            if (File.Exists(savePath))
            {
                File.Delete(savePath);
            }
            Debug.Log("<color=yellow>Đã xóa sạch dữ liệu test!</color>");
        }
    }
}