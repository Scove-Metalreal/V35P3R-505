using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class ScreenshotTool
    {
        [MenuItem("Tools/Screenshot/Take 1x (Game View Size)")]
        public static void Snap1() => TakeShot(1);

        [MenuItem("Tools/Screenshot/Take 2x (HD/Retina)")]
        public static void Snap2() => TakeShot(2);

        [MenuItem("Tools/Screenshot/Take 4x (4K+)")]
        public static void Snap4() => TakeShot(4);

        static void TakeShot(int superSize)
        {
            string date = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            string filename = $"Screenshot_{date}_{superSize}x.png";
        
            ScreenCapture.CaptureScreenshot(filename, superSize);
        
            Debug.Log($"<b>Screenshot Saved:</b> {filename} (at Project Root)");
        
            // Refresh Project view so file appears if you have "Show Extensions" on
            AssetDatabase.Refresh();
        }
    }
}