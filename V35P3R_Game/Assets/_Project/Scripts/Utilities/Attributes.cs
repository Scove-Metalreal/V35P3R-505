using System;
using UnityEngine;

namespace _Project.Scripts.Utilities
{
    public class Attributes
    {
        // 1. Tạo Nút bấm trong Inspector để test hàm
        [AttributeUsage(AttributeTargets.Method)]
        public class GButtonAttribute : PropertyAttribute
        {
            public string ButtonLabel { get; private set; }
            public GButtonAttribute(string label = null)
            {
                ButtonLabel = label;
            }
        }

        // 2. Biến chỉ đọc (ReadOnly) - Chỉ hiện để xem, không cho sửa tay
        public class GReadOnlyAttribute : PropertyAttribute { }

        // 3. Tiêu đề nổi bật (Better Header)
        public class GHeaderAttribute : PropertyAttribute 
        { 
            public string Text;
            public string ColorHex;
        
            public GHeaderAttribute(string text, string colorHex = "#FFD700") // Mặc định màu Vàng
            {
                Text = text;
                ColorHex = colorHex;
            }
        }

        // 4. Đường kẻ phân cách (Separator)
        public class GLineAttribute : PropertyAttribute 
        { 
            public float Height;
            public GLineAttribute(float height = 2f) { Height = height; }
        }
    }
}