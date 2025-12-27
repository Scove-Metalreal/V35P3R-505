using UnityEngine;

namespace _Project.Scripts.Controller
{
    public class C_MovementLogic : MonoBehaviour
    {
        [Header("--- MOVEMENT SETTINGS ---")]
        [SerializeField] private float _walkSpeed = 5f;
        [SerializeField] private float _runSpeed = 8f;
        [SerializeField] private float _jumpForce = 5f;

        [Header("--- LOOK SETTINGS ---")]
        [SerializeField] private float _sensitivity = 15f; // Độ nhạy chuột
        [SerializeField] private float _minPitch = -30f;   // Nhìn xuống tối đa
        [SerializeField] private float _maxPitch = 10f;    // Nhìn lên tối đa

        // Biến lưu góc gật đầu (Pitch) - Cái này cần lưu lại để Clamp
        private float yRotation = 0f;
        private float xRotation = 0f;

        // --- HÀM 1: TÍNH GÓC XOAY CAMERA (CHỈ HEAD) ---
        // Input: Mouse Y
        // Output: Quaternion cho Camera
        public Quaternion CalculateHeadRotation(float mouseY,float mouseX)
        {
            // Cộng dồn góc nhìn (Nhân sensitivity và deltaTime bên ngoài truyền vào)
            // Trừ đi vì chuột lên là nhìn lên (ngược chiều Euler)
            yRotation += mouseX;
            xRotation -= mouseY;

            // Kẹp góc để không gãy cổ
            xRotation = Mathf.Clamp(xRotation, _minPitch, _maxPitch);

            // Trả về góc xoay cục bộ (Local X)
            return  Quaternion.Euler(xRotation, yRotation, 0f);
        }

        // --- HÀM 2: TÍNH VECTOR DI CHUYỂN ---
        // (Giữ nguyên logic ProjectOnPlane cực xịn này)
        public Vector3 CalculateMoveVelocity(Vector2 input, Transform playerTransform, Vector3 gravityUp, bool isRunning)
        {
            // Logic: Di chuyển dựa trên hướng của Player hiện tại
            // Không dùng HeadTransform để tính hướng đi nữa, mà dùng chính PlayerTransform
            // để đảm bảo W luôn là đi về phía trước của ngực
            Vector3 forward = Vector3.ProjectOnPlane(playerTransform.forward, gravityUp).normalized;
            Vector3 right = Vector3.ProjectOnPlane(playerTransform.right, gravityUp).normalized;

            Vector3 moveDir = (forward * input.y + right * input.x).normalized;
            float targetSpeed = isRunning ? _runSpeed : _walkSpeed;
        
            return moveDir * targetSpeed;
        }
    
        public float GetJumpForce() => _jumpForce;
        public float GetSensitivity() => _sensitivity;
    }
}