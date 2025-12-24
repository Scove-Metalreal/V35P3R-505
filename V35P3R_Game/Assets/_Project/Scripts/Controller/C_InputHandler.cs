using UnityEngine;
using UnityEngine.InputSystem;

// Bắt buộc

namespace _Project.Scripts.Controller
{
    public class C_InputHandler : MonoBehaviour
    {
        // --- DATA STORAGE ---
        // Nơi lưu trữ dữ liệu để M_Player lấy dùng
        private Vector2 _moveInput;
        private Vector2 _mouseDelta;
        private bool _jumpInput;
        private bool _runInput;
        private bool _interactInput;
        private bool _abilityInput;
        private bool _crouchInput;

        // --- GETTERS (M_Player sẽ gọi mấy hàm này) ---
        public Vector2 GetMoveInput() => _moveInput;
        public Vector2 GetMouseDelta() => _mouseDelta;
        public bool IsJumpPressed() => _jumpInput;
        public bool IsRunPressed() => _runInput;
        public bool IsInteractPressed() => _interactInput;
        public bool IsAbilityPressed() => _abilityInput;
        public bool IsCrouchPressed() => _crouchInput;

        // =========================================================
        // CÁC HÀM NHẬN TÍN HIỆU TỪ PLAYER INPUT COMPONENT
        // (Tên hàm bắt buộc phải là On + Tên Action)
        // Ví dụ: Action "Move" -> Hàm "OnMove"
        // =========================================================

        // 1. Di chuyển (WASD)
        public void OnMove(InputValue value)
        {
            _moveInput = value.Get<Vector2>();
            // Debug.Log($"Input Move: {_moveInput}"); // Bật dòng này nếu cần test
        }

        // 2. Nhìn chuột (Mouse Delta)
        public void OnLook(InputValue value)
        {
            _mouseDelta = value.Get<Vector2>();
        }

        // 3. Nhảy (Space)
        public void OnJump(InputValue value)
        {
            _jumpInput = value.isPressed;
        }

        // 4. Chạy (Shift)
        public void OnRun(InputValue value)
        {
            _runInput = value.isPressed;
        }

        // 5. Tương tác (E)
        public void OnInteract(InputValue value)
        {
            _interactInput = value.isPressed;
        }

        // 6. Kỹ năng (Q)
        public void OnAbility(InputValue value)
        {
            _abilityInput = value.isPressed;
        }

        // 7. Ngồi (C hoặc Ctrl)
        public void OnCrouch(InputValue value)
        {
            _crouchInput = value.isPressed;
        }
    }
}