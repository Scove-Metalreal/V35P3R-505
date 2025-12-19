using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace Editor
{
    public class NamingValidator : EditorWindow
    {
        private string _report = "Chưa quét. Nhấn nút Scan để bắt đầu.";
        private Vector2 _scrollPos;

        [MenuItem("Tools/Validate Naming Convention")]
        public static void ShowWindow()
        {
            GetWindow<NamingValidator>("Naming Police");
        }

        private void OnGUI()
        {
            GUILayout.Label("KIỂM TRA QUY TẮC ĐẶT TÊN", EditorStyles.boldLabel);
            
            if (GUILayout.Button("SCAN PROJECT NOW", GUILayout.Height(40)))
            {
                RunScan();
            }

            GUILayout.Space(10);
            _scrollPos = GUILayout.BeginScrollView(_scrollPos);
            GUILayout.TextArea(_report);
            GUILayout.EndScrollView();
        }

        private void RunScan()
        {
            int errorCount = 0;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine("KẾT QUẢ QUÉT:");

            // --- 1. CORE ASSETS ---
            // Prefabs
            errorCount += CheckFolder("Assets/_Project/Prefabs", "Pf_", ".prefab", sb);
            
            // Materials
            errorCount += CheckFolder("Assets/_Project/Art/Materials", "Mat_", ".mat", sb);

            // Textures (Hỗ trợ nhiều đuôi ảnh)
            errorCount += CheckFolder("Assets/_Project/Art/Textures", "Tex_", new string[] { ".png", ".jpg", ".tga", ".psd" }, sb);

            // Shaders
            errorCount += CheckFolder("Assets/_Project/Shaders", "Sh_", new string[] { ".shader", ".shadergraph" }, sb);

            // --- 2. AUDIO & ANIMATION ---
            // Audio Music
            errorCount += CheckFolder("Assets/_Project/Art/Audio/Music", "Mus_", new string[] { ".mp3", ".wav", ".ogg" }, sb);

            // Audio SFX
            errorCount += CheckFolder("Assets/_Project/Art/Audio/SFX", "Sfx_", new string[] { ".mp3", ".wav", ".ogg" }, sb);

            // Animator Controllers
            errorCount += CheckFolder("Assets/_Project/Art/Animations", "AC_", ".controller", sb);

            // --- 3. SCENES ---
            // Scenes Chính (Trừ Sandboxes)
            errorCount += CheckFolder("Assets/_Project/Scenes/Main", "Sc_", ".unity", sb);

            // --- 4. SCRIPTS (FULL MVC) ---
            errorCount += CheckFolder("Assets/_Project/Scripts/Controller", "C_", ".cs", sb);
            errorCount += CheckFolder("Assets/_Project/Scripts/Model", "M_", ".cs", sb);
            errorCount += CheckFolder("Assets/_Project/Scripts/View", "V_", ".cs", sb);
            errorCount += CheckFolder("Assets/_Project/Scripts/Interfaces", "I", ".cs", sb);
            // Thêm Managers và Utilities
            errorCount += CheckFolder("Assets/_Project/Scripts/Managers", "Mgr_", ".cs", sb);
            errorCount += CheckFolder("Assets/_Project/Scripts/Utilities", "U_", ".cs", sb);

            if (errorCount == 0)
            {
                _report = "TUYỆT VỜI! 100% file tuân thủ quy tắc đặt tên.";
            }
            else
            {
                sb.Insert(0, $"TÌM THẤY {errorCount} FILE SAI QUY TẮC:\n");
                _report = sb.ToString();
            }
        }

        // Overload cho 1 extension
        private int CheckFolder(string folderPath, string requiredPrefix, string extension, System.Text.StringBuilder sb)
        {
            return CheckFolder(folderPath, requiredPrefix, new string[] { extension }, sb);
        }

        // Overload cho nhiều extension (Vd: Audio có thể là .mp3 hoặc .wav)
        private int CheckFolder(string folderPath, string requiredPrefix, string[] extensions, System.Text.StringBuilder sb)
        {
            if (!Directory.Exists(folderPath)) return 0;

            int count = 0;
            List<string> files = new List<string>();
            
            // Gom file từ tất cả các extension
            foreach(var ext in extensions)
            {
                files.AddRange(Directory.GetFiles(folderPath, "*" + ext, SearchOption.AllDirectories));
            }

            foreach (string file in files)
            {
                string fileName = Path.GetFileName(file);
                // Bỏ qua meta file
                if (fileName.EndsWith(".meta")) continue;

                if (!fileName.StartsWith(requiredPrefix))
                {
                    sb.AppendLine($"[SAI] {fileName}\n      Tại: {file}\n      -> Thiếu tiền tố '{requiredPrefix}'");
                    sb.AppendLine("--------------------------------------------------");
                    count++;
                }
            }
            return count;
        }
    }
}