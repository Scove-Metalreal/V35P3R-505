using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Editor
{
    [InitializeOnLoad]
    public class SmartBootstrapper
    {
        // Đường dẫn đến Scene Boot (Sửa lại nếu bạn đặt tên khác)
        private const string BOOT_SCENE_PATH = "Assets/_Project/Scenes/Main/Sc_00_Boot.unity";
        
        // Menu để Bật/Tắt tính năng này
        private const string MENU_PATH = "Tools/Enable Smart Boot";

        private static bool IsEnabled
        {
            get => EditorPrefs.GetBool("SmartBoot_Enabled", true);
            set => EditorPrefs.SetBool("SmartBoot_Enabled", value);
        }

        static SmartBootstrapper()
        {
            // Đăng ký event khi bấm nút Play
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
        }

        [MenuItem(MENU_PATH)]
        private static void ToggleAction()
        {
            IsEnabled = !IsEnabled;
        }

        [MenuItem(MENU_PATH, true)]
        private static bool ToggleActionValidate()
        {
            Menu.SetChecked(MENU_PATH, IsEnabled);
            return true;
        }

        private static void OnPlayModeChanged(PlayModeStateChange state)
        {
            if (!IsEnabled) return;
            if (state != PlayModeStateChange.ExitingEditMode) return;

            // Nếu Scene hiện tại đã là Boot rồi thì thôi
            string currentScene = EditorSceneManager.GetActiveScene().path;
            if (currentScene == BOOT_SCENE_PATH) return;

            // Kiểm tra xem Scene Boot có tồn tại không
            SceneAsset bootScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(BOOT_SCENE_PATH);
            if (bootScene == null)
            {
                Debug.LogWarning($"[SmartBoot] Không tìm thấy Scene Boot tại: {BOOT_SCENE_PATH}. Vui lòng kiểm tra lại đường dẫn.");
                return;
            }

            // Set Scene Boot làm scene khởi động
            EditorSceneManager.playModeStartScene = bootScene;
            Debug.Log($"<color=green>[SmartBoot]</color> Đang khởi động từ Boot... (Sau đó sẽ load lại {System.IO.Path.GetFileNameWithoutExtension(currentScene)})");
        }
    }
}