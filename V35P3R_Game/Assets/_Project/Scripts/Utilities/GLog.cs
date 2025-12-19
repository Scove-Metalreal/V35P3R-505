using UnityEngine;

namespace _Project.Scripts.Utilities
{

// GLog = Gravity Log
    public static class GLog
    {
        // Log cho Networking (Màu Xanh Cyan)
        public static void Net(object message)
        {
            Debug.Log($"<color=#00FFFF><b>[NETWORK]</b> {message}</color>");
        }

        // Log cho AI (Màu Đỏ Cam)
        public static void AI(object message)
        {
            Debug.Log($"<color=#FF4500><b>[AI]</b> {message}</color>");
        }

        // Log cho UI (Màu Vàng)
        public static void UI(object message)
        {
            Debug.Log($"<color=#FFD700><b>[UI]</b> {message}</color>");
        }

        // Log thành công (Màu Xanh Lá)
        public static void Success(object message)
        {
            Debug.Log($"<color=#00FF00><b>[SUCCESS]</b> {message}</color>");
        }

        // Log quan trọng (Màu Hồng)
        public static void Important(object message)
        {
            Debug.Log($"<color=#FF69B4><b>[ATTENTION]</b> {message}</color>");
        }
    }
}