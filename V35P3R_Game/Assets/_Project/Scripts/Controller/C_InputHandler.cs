using UnityEngine;
using UnityEngine.InputSystem;

// Quan trọng: Cần thư viện này

namespace _Project.Scripts.Controller
{
    public class C_InputHandler : MonoBehaviour
    {
        // Biến để lưu trữ Input Actions đã tạo trong Editor
        private IA_PlayerControl _playerInputActions;

        // Dữ liệu Input sẽ được trả về cho M_Player
        private Vector2 _moveInput;
        private bool _jumpInput;
        private bool _runInput;
        private bool _interactInput;
        private bool _abilityInput;

        // Biến để xử lý xoay Camera (Mouse Look)
        private float _mouseDeltaX;
        private float _mouseDeltaY;

        // --- GETTERS (Trả về dữ liệu Input) ---
        // M_Player sẽ gọi các hàm này để lấy thông tin
        public Vector2 GetMoveInput() => _moveInput;
        public bool IsJumpPressed() => _jumpInput;
        public bool IsRunPressed() => _runInput;
        public bool IsInteractPressed() => _interactInput;
        public bool IsAbilityPressed() => _abilityInput;
        public Vector2 GetMouseDelta() => new Vector2(_mouseDeltaX, _mouseDeltaY);

        private void Awake()
        {
            // Khởi tạo Input Actions
            _playerInputActions = new IA_PlayerControl();
        }

        private void OnEnable()
        {
            // Kích hoạt Input Actions khi script này bật lên
            _playerInputActions.Enable();

            // ĐĂNG KÝ CÁC EVENT XỬ LÝ INPUT TỪ INPUT SYSTEM
            // Khi bấm nút Jump, nó sẽ gọi hàm OnJump
            _playerInputActions.Gameplay.Jump.performed += OnJump;
            _playerInputActions.Gameplay.Jump.canceled += OnJump; // Xử lý nhả nút

            _playerInputActions.Gameplay.Run.performed += OnRun;
            _playerInputActions.Gameplay.Run.canceled += OnRun;

            _playerInputActions.Gameplay.Interact.performed += OnInteract;
            _playerInputActions.Gameplay.Interact.canceled += OnInteract;
        
            _playerInputActions.Gameplay.Ability.performed += OnAbility;
            _playerInputActions.Gameplay.Ability.canceled += OnAbility;

            // Mouse Delta không cần cancel vì nó là analog
            _playerInputActions.Gameplay.Look.performed += OnLook;
        }

        private void OnDisable()
        {
            // Hủy đăng ký event khi script này bị tắt
            _playerInputActions.Gameplay.Jump.performed -= OnJump;
            _playerInputActions.Gameplay.Jump.canceled -= OnJump;

            _playerInputActions.Gameplay.Run.performed -= OnRun;
            _playerInputActions.Gameplay.Run.canceled -= OnRun;

            _playerInputActions.Gameplay.Interact.performed -= OnInteract;
            _playerInputActions.Gameplay.Interact.canceled -= OnInteract;

            _playerInputActions.Gameplay.Ability.performed -= OnAbility;
            _playerInputActions.Gameplay.Ability.canceled -= OnAbility;
        
            _playerInputActions.Gameplay.Look.performed -= OnLook;

            // Tắt hẳn Input Actions
            _playerInputActions.Disable();
        }

        // --- CÁC HÀM XỬ LÝ EVENT TỪ INPUT SYSTEM ---
        // (Các hàm này chỉ cập nhật biến trạng thái Input)

        private void OnMove(InputAction.CallbackContext context)
        {
            _moveInput = context.ReadValue<Vector2>();
        }

        private void OnJump(InputAction.CallbackContext context)
        {
            // true khi bấm, false khi nhả ra
            _jumpInput = context.performed; 
        }

        private void OnRun(InputAction.CallbackContext context)
        {
            _runInput = context.performed;
        }

        private void OnInteract(InputAction.CallbackContext context)
        {
            _interactInput = context.performed;
        }
    
        private void OnAbility(InputAction.CallbackContext context)
        {
            _abilityInput = context.performed;
        }

        private void OnLook(InputAction.CallbackContext context)
        {
            // Lấy Delta X, Y của chuột
            Vector2 mouseDelta = context.ReadValue<Vector2>();
            _mouseDeltaX = mouseDelta.x;
            _mouseDeltaY = mouseDelta.y;
        }
    }
}