using UnityEngine;
using UnityEditor;
using System.IO;

namespace Editor
{
    public class EasyIconMaker : EditorWindow
    {
        [MenuItem("Tools/Easy Icon Maker (Camera)")]
        public static void ShowWindow()
        {
            GetWindow<EasyIconMaker>("Icon Maker");
        }

        private GameObject targetObject;
        private Texture2D previewTexture;
        private int iconSize = 512;
        private Color bgColor = new Color(0, 0, 0, 0); // Trong suốt mặc định

        private void OnGUI()
        {
            GUILayout.Label("CHỤP ẢNH MODEL THÀNH ICON", EditorStyles.boldLabel);

            targetObject = (GameObject)EditorGUILayout.ObjectField("Target Prefab", targetObject, typeof(GameObject), true);
            iconSize = EditorGUILayout.IntSlider("Size (px)", iconSize, 128, 1024);
            bgColor = EditorGUILayout.ColorField("Background", bgColor);

            if (GUILayout.Button("Preview (Xem thử)", GUILayout.Height(30)))
            {
                if (targetObject != null) CaptureIcon(false);
            }

            if (previewTexture != null)
            {
                GUILayout.Label(previewTexture, GUILayout.Width(200), GUILayout.Height(200));
            }

            if (GUILayout.Button("SAVE PNG", GUILayout.Height(40)))
            {
                if (targetObject != null) CaptureIcon(true);
            }
            
            GUILayout.Space(10);
            GUILayout.Label("Mẹo: Tool sẽ dùng Camera của Scene View để chụp.\nHãy xoay Camera trong Scene sao cho góc nhìn đẹp nhất.", EditorStyles.helpBox);
        }

        private void CaptureIcon(bool saveFile)
        {
            // Tạo Camera ảo để chụp
            GameObject camObj = new GameObject("IconCam");
            Camera cam = camObj.AddComponent<Camera>();
            
            // Đồng bộ vị trí với Scene View Camera để dễ canh góc
            Camera sceneCam = SceneView.lastActiveSceneView.camera;
            cam.transform.position = sceneCam.transform.position;
            cam.transform.rotation = sceneCam.transform.rotation;
            
            // Cấu hình Camera
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = bgColor;
            cam.fieldOfView = sceneCam.fieldOfView;

            // Tạo RenderTexture tạm
            RenderTexture rt = new RenderTexture(iconSize, iconSize, 24);
            cam.targetTexture = rt;
            
            // Ẩn tất cả object khác, chỉ hiện targetObject? 
            // Cách đơn giản nhất: Dịch chuyển targetObject và Camera ra xa vô tận để chụp
            Vector3 oldPos = targetObject.transform.position;
            Quaternion oldRot = targetObject.transform.rotation;
            
            // Đưa ra chỗ vắng (Y = 10000)
            Vector3 isolatePos = new Vector3(0, 10000, 0);
            targetObject.transform.position = isolatePos;
            
            // Đưa Camera theo
            Vector3 offset = cam.transform.position - oldPos; // Khoảng cách cũ
            cam.transform.position = isolatePos + offset;
            
            // Chụp!
            cam.Render();

            // Đọc pixels
            RenderTexture.active = rt;
            Texture2D screenShot = new Texture2D(iconSize, iconSize, TextureFormat.ARGB32, false);
            screenShot.ReadPixels(new Rect(0, 0, iconSize, iconSize), 0, 0);
            screenShot.Apply();

            // Trả vật thể về chỗ cũ
            targetObject.transform.position = oldPos;
            targetObject.transform.rotation = oldRot;

            // Dọn dẹp
            cam.targetTexture = null;
            RenderTexture.active = null;
            DestroyImmediate(rt);
            DestroyImmediate(camObj);

            previewTexture = screenShot;

            if (saveFile)
            {
                byte[] bytes = screenShot.EncodeToPNG();
                string path = EditorUtility.SaveFilePanel("Save Icon", "Assets/_Project/Art/Textures/UI_Icons", targetObject.name + "_Icon", "png");
                if (path.Length != 0)
                {
                    File.WriteAllBytes(path, bytes);
                    AssetDatabase.Refresh();
                    Debug.Log("Đã lưu Icon tại: " + path);
                }
            }
        }
    }
}