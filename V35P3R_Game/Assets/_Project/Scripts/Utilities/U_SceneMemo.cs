using UnityEditor;
using UnityEngine;

namespace _Project.Scripts.Utilities
{
    // Script này giúp để lại ghi chú trực quan trong Scene
    [ExecuteInEditMode]
    public class U_SceneMemo : MonoBehaviour
    {
        [Header("Content")]
        [TextArea(3, 10)]
        public string note = "Viết ghi chú ở đây...";
        
        [Header("Appearance")]
        public Color textColor = Color.yellow;
        [Range(10, 50)] public int fontSize = 14; // Dùng int cho size chữ chuẩn hơn
        public bool showAlways = true;
        public bool showBackground = true; // Nền đen mờ giúp dễ đọc

        [Header("Positioning")]
        public Vector3 offset = new Vector3(0, 2f, 0); // Đẩy chữ lên cao
        public bool drawLine = true; // Vẽ dây nối từ chữ xuống object
        public float maxViewDistance = 50f; // Ẩn khi ở xa để đỡ rối

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!showAlways) return;
            DrawMemo();
        }

        private void OnDrawGizmosSelected()
        {
            if (showAlways) return; // Đã vẽ rồi thì thôi
            DrawMemo();
        }

        private void DrawMemo()
        {
            if (string.IsNullOrEmpty(note)) return;

            // 1. Kiểm tra khoảng cách Camera
            var view = SceneView.currentDrawingSceneView;
            if (view == null || view.camera == null) return;

            float dist = Vector3.Distance(view.camera.transform.position, transform.position);
            if (dist > maxViewDistance) return;

            Vector3 worldPos = transform.position + offset;

            // 2. Vẽ đường nối (Connector)
            if (drawLine)
            {
                Gizmos.color = textColor;
                Gizmos.DrawLine(transform.position, worldPos);
                Gizmos.DrawSphere(transform.position, 0.15f); // Cục neo nhỏ tại vị trí gốc
            }

            // 3. Vẽ chữ
            DrawString(note, worldPos, textColor);
        }

        private void DrawString(string text, Vector3 worldPos, Color color)
        {
            Handles.BeginGUI();
            
            var view = SceneView.currentDrawingSceneView;
            if (view != null && view.camera != null)
            {
                Vector3 screenPos = view.camera.WorldToScreenPoint(worldPos);
                
                // Chỉ vẽ nếu ở phía trước camera
                if (screenPos.z > 0) 
                {
                    // Setup Style cho chữ
                    GUIStyle style = new GUIStyle(GUI.skin.label);
                    style.normal.textColor = color;
                    style.fontSize = fontSize;
                    style.alignment = TextAnchor.MiddleCenter;
                    style.fontStyle = FontStyle.Bold;

                    // Tính kích thước khung chứa chữ
                    Vector2 size = style.CalcSize(new GUIContent(text));
                    // Đảo ngược Y vì GUI tọa độ khác World, cộng thêm padding
                    float padding = 6f;
                    Rect rect = new Rect(
                        screenPos.x - (size.x / 2), 
                        -screenPos.y + view.position.height, 
                        size.x, 
                        size.y
                    );

                    // Vẽ nền đen mờ (Background Box)
                    if (showBackground)
                    {
                        Rect bgRect = new Rect(rect.x - padding, rect.y - padding, rect.width + padding * 2, rect.height + padding * 2);
                        EditorGUI.DrawRect(bgRect, new Color(0, 0, 0, 0.7f));
                    }

                    // Vẽ chữ
                    GUI.Label(rect, text, style);
                }
            }

            Handles.EndGUI();
        }
#endif
    }
}