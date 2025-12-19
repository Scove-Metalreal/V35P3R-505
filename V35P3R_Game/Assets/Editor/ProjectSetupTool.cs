using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Editor
{
    public class ProjectSetupTool
    {
        // Tên gốc của dự án
        private static string ROOT = "_Project";

        [MenuItem("Tools/Setup Project Structure (Full)")]
        public static void SetupProject()
        {
            // 1. Tạo Folders & Readmes hướng dẫn
            CreateDirectories();

            // 2. Tạo Scene chuẩn (Boot, Menu, Game)
            CreateStandardScenes();

            // 3. Tạo Gitignore (nếu chưa có)
            CreateGitIgnore();

            AssetDatabase.Refresh();
            Debug.Log("<color=#00FF00><b>[SUCCESS]</b> Project Structure setup complete!</color>");
        }

        private static void CreateDirectories()
        {
            // Cấu trúc thư mục + Nội dung Readme tương ứng
            var directories = new Dictionary<string, string>
            {
                // --- ART ASSETS ---
                { $"{ROOT}/Art/Animations", "Chứa .anim clips và Animator Controllers (Prefix: AC_)." },
                { $"{ROOT}/Art/Audio/Music", "Nhạc nền (BGM). Prefix: Mus_" },
                { $"{ROOT}/Art/Audio/SFX", "Âm thanh hiệu ứng (One-shot). Prefix: Sfx_" },
                { $"{ROOT}/Art/Materials", "Prefix: Mat_. Dùng Shader Graph hoặc URP Lit." },
                { $"{ROOT}/Art/Models/Characters", "File .fbx nhân vật. Nhớ set Rig thành Humanoid." },
                { $"{ROOT}/Art/Models/Environment", "File .fbx môi trường (Tường, sàn, cột)." },
                { $"{ROOT}/Art/Models/Props", "Đồ vật nhỏ (Súng, Hộp đạn, Ghế)." },
                { $"{ROOT}/Art/Textures", "Texture gốc (.png, .tga). Prefix: Tex_" },
                { $"{ROOT}/Art/VFX", "Particle Systems, VFX Graph assets." },

                // --- PREFABS (QUAN TRỌNG) ---
                { $"{ROOT}/Prefabs/Characters", "Prefab Người chơi và Quái vật. Prefix: Pf_Char_" },
                { $"{ROOT}/Prefabs/Environment", "Prefab Môi trường tĩnh. Prefix: Pf_Env_" },
                { $"{ROOT}/Prefabs/Gameplay", "Item, Projectiles (Đạn), Traps. Prefix: Pf_Game_" },
                { $"{ROOT}/Prefabs/Systems", "NetworkManager, GameManager, AudioMgr. Prefix: Pf_Sys_" },
                { $"{ROOT}/Prefabs/UI", "Các Popups, HUD, Menu Panels. Prefix: Pf_UI_" },

                // --- SCENES ---
                { $"{ROOT}/Scenes/Main", "Các Scene chính thức của game (Build Settings)." },
                { $"{ROOT}/Scenes/Sandboxes/Scove", "Khu vực test của Scove. Đừng ai đụng vào!" },
                { $"{ROOT}/Scenes/Sandboxes/Duy", "Khu vực test của Duy. (Physics, Netcode)" },
                { $"{ROOT}/Scenes/Sandboxes/Dang", "Khu vực test của Đăng. (Skills, Gameplay)" },
                { $"{ROOT}/Scenes/Sandboxes/Tuan", "Khu vực test của Tuấn. (Map, Audio)" },

                // --- SCRIPTS (MVC) ---
                { $"{ROOT}/Scripts/Controller", "CONTROLLER (C_): Xử lý Input, Logic, giao tiếp giữa Model và View.\nKHÔNG lưu dữ liệu cứng." },
                { $"{ROOT}/Scripts/Model", "MODEL (M_): Chứa dữ liệu (Data), Stats, Settings.\nKHÔNG tham chiếu UI, KHÔNG xử lý Input." },
                { $"{ROOT}/Scripts/View", "VIEW (V_): Hiển thị UI, Animation, VFX, Âm thanh.\nCHỈ nhận lệnh từ Controller, KHÔNG tự quyết định logic." },
                { $"{ROOT}/Scripts/Interfaces", "INTERFACE (I): Các bản thiết kế chung (IInteractable, IDamageable)." },
                { $"{ROOT}/Scripts/Managers", "MANAGER (Mgr_): Quản lý toàn cục (GameLoop, NetworkRunner)." },
                { $"{ROOT}/Scripts/Utilities", "UTILITIES (U_): Các hàm hỗ trợ tĩnh (Math, String format)." },

                // --- OTHERS ---
                { $"{ROOT}/Settings", "URP Settings, Input Action Assets, Physics Materials." },
                { $"{ROOT}/Shaders", "Shader Graph files. Prefix: Sh_" },
                { "ThirdParty", "Assets mua từ Store hoặc tải trên mạng. KHÔNG SỬA CODE TRONG NÀY." },
                { "Plugins", "SDKs (Steamworks, Firebase, etc.)" }
            };

            foreach (var kvp in directories)
            {
                string fullPath = Path.Combine(Application.dataPath, kvp.Key);
                if (!Directory.Exists(fullPath))
                {
                    Directory.CreateDirectory(fullPath);
                }

                // Tạo file Readme.txt trong mỗi folder
                string readmePath = Path.Combine(fullPath, "!READ_ME.txt");
                if (!File.Exists(readmePath))
                {
                    File.WriteAllText(readmePath, kvp.Value);
                }
            }
        }

        private static void CreateStandardScenes()
        {
            string sceneFolder = Path.Combine(Application.dataPath, $"{ROOT}/Scenes/Main");
            if (!Directory.Exists(sceneFolder)) Directory.CreateDirectory(sceneFolder);

            // Danh sách scene cần tạo
            string[] sceneNames = { "00_Boot", "01_Menu", "02_Game" };

            foreach (string sceneName in sceneNames)
            {
                string path = $"Assets/{ROOT}/Scenes/Main/Sc_{sceneName}.unity";
                
                // Chỉ tạo nếu chưa tồn tại để tránh ghi đè scene cũ
                if (!File.Exists(path))
                {
                    var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
                    EditorSceneManager.SaveScene(scene, path);
                    Debug.Log($"Created Scene: {path}");
                }
            }
        }

        private static void CreateGitIgnore()
        {
            string projectRoot = Directory.GetParent(Application.dataPath).FullName;
            string gitIgnorePath = Path.Combine(projectRoot, ".gitignore");

            if (!File.Exists(gitIgnorePath))
            {
                string content = 
@"# Unity generated
/[Ll]ibrary/
/[Tt]emp/
/[Oo]bj/
/[Bb]uild/
/[Bb]uilds/
/[Ll]ogs/
/[Uu]ser[Ss]ettings/

# Visual Studio / Rider
.vs/
.idea/
*.csproj
*.sln
*.suo
*.tmp
*.user
*.userprefs
*.pidb
*.booproj
*.svd
*.pdb
*.opendb
*.VC.db

# OS generated
.DS_Store
Thumbs.db
";
                File.WriteAllText(gitIgnorePath, content);
                Debug.Log("Created .gitignore file at project root.");
            }
        }
    }
}