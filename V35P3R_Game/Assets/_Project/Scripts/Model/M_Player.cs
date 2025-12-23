using _Project.Scripts.Controller;
using _Project.Scripts.Interfaces;
using _Project.Scripts.Managers;
using _Project.Scripts.View;
using UnityEngine;

namespace _Project.Scripts.Model
{
    [RequireComponent(typeof(Rigidbody))]
    public class M_Player : MonoBehaviour
    {
        [Header("--- COMPONENTS (WIRING) ---")]
        [SerializeField] private C_InputHandler _inputHandler;
        [SerializeField] private C_GravityLogic _gravityLogic;
        [SerializeField] private C_MovementLogic _moveLogic;

        [Header("--- VISUALS ---")]
        [SerializeField] private Transform _headTransform; // Gắn object Camera vào đây
        [SerializeField] private Transform _bodyModel;     // Gắn object Model phi hành gia vào đây
        [SerializeField] private V_PlayerVisual _visual;    // Gắn object V_PlayerVisual vào đây
        
        [Header("--- INTERACTION WIRING ---")]
        [SerializeField] private C_InteractionLogic _interactLogic; // Kéo script logic vào
        [SerializeField] private Transform _holdPosition;           // Kéo object HoldPosition vào
        
        [Header("--- STATS WIRING ---")]
        [SerializeField] private C_StatsLogic _statsLogic; // Kéo script Logic Stats vào
        [SerializeField] private V_PlayerHUD _hudView;     // Kéo Canvas HUD vào

        // --- STATE (TRẠNG THÁI) ---
        private Rigidbody _rb;
        private C_GravityLogic.GravityDirection _currentGravityDir = C_GravityLogic.GravityDirection.Down;
        // Biến lưu trữ vật đang cầm trên tay
        private Item_Scrap _heldItem = null;
    
        // Biến lưu vật đang nhìn thấy (để hiện UI)
        private IInteractable _currentTarget = null; 
        
        [SerializeField] private float _currentHealth;
        [SerializeField] private float _currentOxygen;
        private bool _isDead = false;


        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        
            // Setup Rigidbody chuẩn cho Physics Movement
            _rb.useGravity = false; // Tắt trọng lực mặc định của Unity để dùng trọng lực tùy chỉnh
            _rb.freezeRotation = true; // Tắt vật lý xoay tự do, ta sẽ xoay bằng code
            _rb.interpolation = RigidbodyInterpolation.Interpolate; // Giúp di chuyển mượt hơn
            _rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            
            if (_inputHandler == null) _inputHandler = GetComponent<C_InputHandler>();
            if (_headTransform == null) _headTransform = GetComponentInChildren<Camera>().transform;
        }

        private void Start()
        {
            // Khóa chuột ẩn đi
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        
            // Khởi tạo trọng lực ban đầu
            UpdateGravityState(C_GravityLogic.GravityDirection.Down);
            
            if (_statsLogic)
            {
                _currentHealth = _statsLogic.GetMaxHealth();
                _currentOxygen = _statsLogic.GetMaxOxygen();
            }
            
        
            // Update UI lần đầu
            _hudView.UpdateHealth(_currentHealth, _statsLogic.GetMaxHealth());
            _hudView.UpdateOxygen(_currentOxygen, _statsLogic.GetMaxOxygen());
        }

        // --- GAME LOOP: XỬ LÝ LOGIC & VISUAL ---
        private void Update()
        {
            if (_isDead) return;

            // 1. Các logic game (Chỉ gọi 1 lần thôi nhé)
            HandleGravitySwitch();
            // 2. Xử lý Xoay Camera & Body (Logic mới của bạn)
            HandleCameraLook();
            HandleSurvivalStats();

            // 3. Visual Animation
            if (_visual != null)
            {
                _visual.UpdateMovementAnim(
                    _inputHandler.GetMoveInput(), 
                    _inputHandler.IsRunPressed(), 
                    _inputHandler.IsCrouchPressed()
                );
            }
        }

        // --- PHYSICS LOOP: XỬ LÝ DI CHUYỂN & VẬT LÝ ---
        private void FixedUpdate()
        {
            if (_isDead) return;

            // --- BƯỚC 1: TRỌNG LỰC ---
            Vector3 gravityForce = _gravityLogic.CalculateGravityForce(_currentGravityDir);
            Vector3 gravityUp = -gravityForce.normalized;
            _rb.AddForce(gravityForce, ForceMode.Acceleration);

            // --- BƯỚC 2: XOAY NHÂN VẬT (QUAN TRỌNG NHẤT) ---
        
            // A. Căn chỉnh theo trọng lực (Gravity Alignment)
            // Tạo một góc xoay từ hướng Up hiện tại -> hướng Up của trọng lực
            Quaternion targetUpRotation = Quaternion.FromToRotation(transform.up, gravityUp) * transform.rotation;
        
            // Slerp để nghiêng người mượt mà khi đổi hướng trọng lực (50f là tốc độ nghiêng)
            Quaternion blendedRotation = Quaternion.Slerp(transform.rotation, targetUpRotation, 50f * Time.fixedDeltaTime);

            // B. Xoay trái phải theo chuột (Mouse Yaw)
            float mouseX = _inputHandler.GetMouseDelta().x * _moveLogic.GetSensitivity() * Time.fixedDeltaTime;
        
            // Tạo góc xoay quanh trục Y (trục Up) của chính nhân vật
            Quaternion yawRotation = Quaternion.Euler(0f, mouseX, 0f);

            // Kết hợp: Xoay theo trọng lực trước -> Rồi xoay theo chuột
            _rb.MoveRotation(blendedRotation * yawRotation);

            // --- BƯỚC 3: DI CHUYỂN ---
            Vector2 moveInput = _inputHandler.GetMoveInput();
            bool isRunning = _inputHandler.IsRunPressed();

            // Truyền Transform của Player vào để tính hướng đi (W = Forward của Player)
            Vector3 moveVelocity = _moveLogic.CalculateMoveVelocity(moveInput, transform, gravityUp, isRunning);
        
            _rb.MovePosition(_rb.position + moveVelocity * Time.fixedDeltaTime);

            // --- BƯỚC 4: NHẢY ---
            if (_inputHandler.IsJumpPressed())
            {
                _rb.AddForce(gravityUp * _moveLogic.GetJumpForce(), ForceMode.Impulse);
            }
        }
        
        
        // --- LOGIC HELPER ---
        private void HandleGravitySwitch()
        {
            // Lấy các input cần thiết
            bool interact = _inputHandler.IsInteractPressed();
            bool ability = _inputHandler.IsAbilityPressed();
            Vector2 move = _inputHandler.GetMoveInput();
        
            // Chỉ đổi trọng lực khi giữ Ctrl (Giả sử bạn vẫn muốn giữ logic Ctrl)
            // Nếu muốn bỏ Ctrl thì xóa điều kiện này
            // Ở đây tôi giả định dùng Ability Key (Q) để trigger đổi trọng lực cho hiện đại
            // Hoặc giữ nguyên logic cũ:
            if (UnityEngine.InputSystem.Keyboard.current.leftCtrlKey.isPressed) 
            {
                C_GravityLogic.GravityDirection newDir = _gravityLogic.GetNewDirectionFromInput(_currentGravityDir, move, interact, ability);
            
                if (newDir != _currentGravityDir)
                {
                    UpdateGravityState(newDir);
                }
            }
        }

        private void UpdateGravityState(C_GravityLogic.GravityDirection newDir)
        {
            _currentGravityDir = newDir;
            // Reset vận tốc khi đổi trọng lực để tránh bị văng quá xa
            _rb.linearVelocity = Vector3.zero; 
        
            Debug.Log($"Gravity changed to: {newDir}");
        }
        
        
        private void HandleInteraction()
        {
            // 1. Luôn quét xem đang nhìn cái gì
            _currentTarget = _interactLogic.RaycastCheck(_headTransform);

            // TODO: Gọi UI View để hiện dòng chữ "Press E to pick up..."
            if (_currentTarget != null) 
            {
                // Debug.Log(_currentTarget.GetInteractionPrompt()); 
            }

            // 2. Nếu bấm E và đang nhìn thấy vật -> Tương tác
            if (_inputHandler.IsInteractPressed() && _currentTarget != null)
            {
                // Nếu tay đang rảnh thì mới nhặt
                if (_heldItem == null)
                {
                    _currentTarget.OnInteract(this);
                }
            }
        }
        
        private void HandleDrop()
        {
            // Nếu đang cầm đồ mà bấm G (Giả sử map nút G là Drop, hoặc bấm chuột trái)
            // Bạn cần map thêm nút Drop trong Input System hoặc dùng tạm nút nào đó
            // Ví dụ: Bấm E lần nữa để thả
        
            // (Logic tạm: Nếu đang cầm đồ mà bấm E thì thả ra)
            /*
            if (_heldItem != null && _inputHandler.IsInteractPressed())
            {
                 DropItem();
            }
            */
        }
        
        private void HandleCameraLook()
        {
            // Lấy Mouse Y (Lên xuống)
            float mouseY = _inputHandler.GetMouseDelta().y * _moveLogic.GetSensitivity() * Time.deltaTime;
        
            // Tính toán góc gật đầu
            Quaternion headRot = _moveLogic.CalculateHeadRotation(mouseY);
        
            // Áp dụng vào Camera
            _headTransform.localRotation = headRot;
        }
        
        public void PickupItem(Item_Scrap item)
        {
            if (_heldItem != null) return; // Đang cầm rồi thì thôi

            _heldItem = item;
        
            // Gọi Logic để gắn vật vào tay (Visual & Physics)
            _interactLogic.AttachItemToHand(item.transform, _holdPosition);
        
            // Logic game: Có thể giảm tốc độ di chuyển vì nặng
            // _moveLogic.SetWeightPenalty(item.GetWeight()); (Nếu có tính năng này)
        
            Debug.Log($"Picked up: {item.name}");
        }

        public void DropItem()
        {
            if (_heldItem == null) return;

            // Gọi Logic để tách vật ra (Visual & Physics)
            // Ném về phía trước Camera một chút
            _interactLogic.DetachItem(_heldItem.transform, _headTransform.forward * 5f);
        
            _heldItem = null;
            Debug.Log("Dropped item");
        }
        
        private void HandleSurvivalStats()
        {
            // 1. Trừ Oxi theo thời gian
            _currentOxygen = _statsLogic.CalculateOxygenDrain(_currentOxygen, Time.deltaTime);

            // 2. Kiểm tra ngạt thở (Hết Oxi thì trừ máu)
            float suffocationDmg = _statsLogic.GetSuffocationDamage(_currentOxygen);
            if (suffocationDmg != 0)
            {
                // Trừ máu theo thời gian (nhân deltaTime vì dmg tính theo giây)
                ApplyDamage(Mathf.Abs(suffocationDmg) * Time.deltaTime);
            }

            // 3. Cập nhật UI
            _hudView.UpdateOxygen(_currentOxygen, _statsLogic.GetMaxOxygen());
        
            // (Optional) Chỉnh interaction text từ logic cũ sang UI mới
            if (_currentTarget != null)
                _hudView.SetInteractionText(_currentTarget.GetInteractionPrompt());
            else
                _hudView.SetInteractionText("");
        }

        // Hàm nhận sát thương (Public để Quái/Môi trường gọi)
        public void ApplyDamage(float amount)
        {
            if (_isDead) return;

            // Gọi Logic tính toán trừ máu (amount dương -> trừ, amount âm -> hồi - tùy quy ước)
            // Ở đây tôi quy ước: ApplyDamage nhận số dương là bị đau
            _currentHealth = _statsLogic.CalculateHealthChange(_currentHealth, -amount);

            // Update UI
            _hudView.UpdateHealth(_currentHealth, _statsLogic.GetMaxHealth());

            // Check chết
            if (_statsLogic.IsDead(_currentHealth))
            {
                Die();
            }
        }

        private void Die()
        {
            _isDead = true;
            Debug.Log("PLAYER DIED");
        
            // Gọi UI Game Over
            // _hudView.ShowGameOver();
        
            // Tắt di chuyển physics
            _rb.isKinematic = true;
        }
    }
}