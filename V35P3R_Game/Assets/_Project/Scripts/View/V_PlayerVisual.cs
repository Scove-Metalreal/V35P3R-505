using UnityEngine;

namespace _Project.Scripts.View
{
    [RequireComponent(typeof(Animator))]
    public class V_PlayerVisual : MonoBehaviour
    {
        [Header("--- ANIMATION SETTINGS ---")]
        [Tooltip("Độ mượt khi chuyển animation (Damping)")]
        [SerializeField] private float _dampTime = 0.1f; 

        // --- HASHING (Tối ưu hiệu năng thay vì dùng string) ---
        private int _animIDVelX;
        private int _animIDVelZ;
        private int _animIDCrouch;
        private int _animIDInteract; // Dùng cho đấm/tương tác

        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        
            // Convert tên tham số trong Animator sang ID số nguyên
            // Đảm bảo trong Animator bạn đã tạo 2 Float: "VelocityX", "VelocityZ" và 1 Bool "isCrouch"
            _animIDVelX = Animator.StringToHash("VelocityX"); 
            _animIDVelZ = Animator.StringToHash("VelocityZ");
            _animIDCrouch = Animator.StringToHash("isCrouch");
            _animIDInteract = Animator.StringToHash("Interact");
        }

        // --- HÀM CẬP NHẬT TRẠNG THÁI DI CHUYỂN ---
        // M_Player sẽ gọi hàm này mỗi khung hình
        public void UpdateMovementAnim(Vector2 moveInput, bool isRunning, bool isCrouching)
        {
            // 1. Tính toán giá trị đích (Target Value) cho Blend Tree
            // Nếu chạy: Max = 2, Đi bộ: Max = 1 (Tùy setup BlendTree của bạn)
            // Blend Tree thường setup: 0 (Idle), 0.5 (Walk), 1 (Run)
            float targetSpeed = isRunning ? 1f : 0.5f;
        
            // Nếu không bấm nút di chuyển thì về 0
            if (moveInput == Vector2.zero) targetSpeed = 0f;

            // Tách Vector input thành X và Z cục bộ để Animator hiểu (Strafing)
            float targetX = moveInput.x * targetSpeed;
            float targetZ = moveInput.y * targetSpeed;

            // 2. Đẩy vào Animator (Dùng Damp để làm mượt tự động)
            // Animator.SetFloat đã có sẵn chức năng làm mượt (dampTime), không cần code tay công thức Mathf.MoveTowards
            _animator.SetFloat(_animIDVelX, targetX, _dampTime, Time.deltaTime);
            _animator.SetFloat(_animIDVelZ, targetZ, _dampTime, Time.deltaTime);

            // 3. Cập nhật trạng thái ngồi
            _animator.SetBool(_animIDCrouch, isCrouching);
        }

        // --- HÀM KÍCH HOẠT HÀNH ĐỘNG (TRIGGER) ---
        // Gọi khi đấm hoặc tương tác
        public void TriggerInteractAnim()
        {
            _animator.SetTrigger(_animIDInteract);
        }
    }
}