using UnityEngine;

namespace _Project.Scripts.Controller
{
    public class C_MovementLogic : MonoBehaviour
    {
        [Header("--- SPEED SETTINGS ---")]
        [SerializeField] private float _walkSpeed = 2.5f;
        [SerializeField] private float _runSpeed = 5f;
        [SerializeField] private float _jumpForce = 5f; // Lực nhảy (Impulse)

        [Header("--- LOOK SETTINGS ---")]
        [SerializeField] private float _sensitivity = 0.1f; // Độ nhạy chuột
        [SerializeField] private float _viewClampY = 80f;   // Giới hạn nhìn lên/xuống

        // Biến lưu góc xoay hiện tại của Camera (X Rotation)
        private float _currentCameraXRotation = 0f;

        // --- HÀM 1: TÍNH TOÁN VECTOR DI CHUYỂN ---
        // Logic: Di chuyển phải luôn vuông góc với hướng trọng lực (Up Direction)
        public Vector3 CalculateMoveVelocity(Vector2 input, Transform headTransform, Vector3 gravityUp, bool isRunning)
        {
            // 1. Xác định hướng "Phía trước" và "Bên phải" dựa trên hướng nhìn của đầu
            // và chiếu nó lên mặt phẳng vuông góc với trọng lực (ProjectOnPlane)
            Vector3 forward = Vector3.ProjectOnPlane(headTransform.forward, gravityUp).normalized;
            Vector3 right = Vector3.ProjectOnPlane(headTransform.right, gravityUp).normalized;

            // 2. Tổng hợp hướng đi từ Input WASD
            Vector3 moveDir = (forward * input.y + right * input.x).normalized;

            // 3. Nhân với tốc độ
            float targetSpeed = isRunning ? _runSpeed : _walkSpeed;
        
            // Trả về Velocity cần thiết (để M_Player dùng rb.MovePosition hoặc rb.velocity)
            return moveDir * targetSpeed;
        }

        // --- HÀM 2: TÍNH TOÁN GÓC XOAY CAMERA (MOUSE LOOK) ---
        // Logic: Xoay đầu lên xuống, giới hạn góc kẹp
        public Quaternion CalculateHeadRotation(Vector2 mouseDelta, Transform currentHeadTransform)
        {
            // mouseDelta.x = Quay trái phải (Body lo), mouseDelta.y = Quay lên xuống (Head lo)
            float mouseY = mouseDelta.y * _sensitivity;

            // Trừ đi vì kéo chuột xuống là nhìn lên (Inverted logic thông thường)
            _currentCameraXRotation -= mouseY;
        
            // Kẹp góc nhìn để không bị gãy cổ (Clamp)
            _currentCameraXRotation = Mathf.Clamp(_currentCameraXRotation, -_viewClampY, _viewClampY);

            // Trả về Quaternion góc xoay địa phương (Local Rotation)
            return Quaternion.Euler(_currentCameraXRotation, 0f, 0f);
        }

        // --- HÀM 3: TÍNH TOÁN XOAY THÂN NGƯỜI (BODY ROTATION) ---
        // Logic: Thân người xoay theo chuột trái/phải + Xoay theo hướng trọng lực
        public Quaternion CalculateBodyRotation(Vector2 mouseDelta, Transform currentBodyTransform, Vector3 gravityUp)
        {
            // Xử lý xoay ngang theo chuột
            float mouseX = mouseDelta.x * _sensitivity;
        
            // Lấy góc xoay hiện tại cộng thêm mouseX (Xoay quanh trục Y cục bộ - tức là trục Up của trọng lực)
            // Lưu ý: Toán tử * Quaternion tương đương với việc cộng góc xoay
            Quaternion horizontalRotation = Quaternion.AngleAxis(mouseX, Vector3.up);
        
            // Logic này giả định M_Player đã xử lý việc xoay toàn bộ Body theo trọng lực rồi.
            // Script này chỉ tính lượng xoay thêm do chuột.
            return currentBodyTransform.localRotation * horizontalRotation;
        }
    
        public float GetJumpForce() => _jumpForce;
    }
}