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
    
        // Lưu trữ Quaternion đích để xoay nhân vật mượt mà
        private Quaternion _targetGravityRotation = Quaternion.identity;
        
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
        }

        private void Start()
        {
            // Khóa chuột ẩn đi
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        
            // Khởi tạo trọng lực ban đầu
            UpdateGravityState(C_GravityLogic.GravityDirection.Down);
            
            // KHỞI TẠO CHỈ SỐ
            _currentHealth = _statsLogic.GetMaxHealth();
            _currentOxygen = _statsLogic.GetMaxOxygen();
        
            // Update UI lần đầu
            _hudView.UpdateHealth(_currentHealth, _statsLogic.GetMaxHealth());
            _hudView.UpdateOxygen(_currentOxygen, _statsLogic.GetMaxOxygen());
        }

        // --- GAME LOOP: XỬ LÝ LOGIC & VISUAL ---
        private void Update()
        {
            if (_isDead) return; // Chết rồi thì không làm gì cả (hiện bảng Game Over)

            HandleGravitySwitch();
            HandleInteraction(); // Update Interaction UI ở đây
            HandleSurvivalStats();
            // 1. Xử lý Input đổi trọng lực (Logic Game)
            HandleGravitySwitch();
            HandleInteraction();
            HandleDrop();

            // 2. Xử lý Xoay Camera (Mouse Look)
            // Lấy delta chuột từ Input
            Vector2 mouseDelta = _inputHandler.GetMouseDelta();
        
            // Gọi Logic tính toán góc xoay đầu
            Quaternion headRot = _moveLogic.CalculateHeadRotation(mouseDelta, _headTransform);
            _headTransform.localRotation = headRot;

            // Xoay thân người sang trái/phải theo chuột (Body Yaw)
            // Lưu ý: Ta xoay quanh trục Y cục bộ (Local Up) của nhân vật
            if (_bodyModel != null)
            {
                // Nếu muốn xoay model riêng
                // _bodyModel.localRotation = ... (Tùy logic animation)
                // Nhưng thường với FPS, ta xoay cả cục Player trong FixedUpdate bên dưới
            }
        }

        // --- PHYSICS LOOP: XỬ LÝ DI CHUYỂN & VẬT LÝ ---
        private void FixedUpdate()
        {
            // 1. Áp dụng Trọng lực (Force)
            Vector3 gravityForce = _gravityLogic.CalculateGravityForce(_currentGravityDir);
            _rb.AddForce(gravityForce, ForceMode.Acceleration);

            // 2. Xoay Nhân vật theo hướng trọng lực (Rotation)
            // Lấy Input xoay ngang (Mouse X) để cộng vào góc xoay của nhân vật
            float mouseX = _inputHandler.GetMouseDelta().x;
            float sensitivity = 0.1f; // Nên lấy từ Settings, tạm thời để cứng hoặc lấy từ MoveLogic
        
            // Tính toán góc xoay mục tiêu: Góc trọng lực * Góc xoay ngang do chuột
            // Logic: Xoay toàn bộ Rigidbody để chân luôn hướng về phía trọng lực
            Quaternion targetRot = _gravityLogic.CalculateTargetRotation(_currentGravityDir);
        
            // Kết hợp xoay theo chuột (quanh trục Y của trọng lực)
            // Phần này hơi phức tạp: Ta muốn giữ nguyên hướng nhìn trái phải hiện tại tương đối với mặt đất mới
            // Trong bản đơn giản này, ta ưu tiên xoay về trọng lực trước
        
            // Dùng MoveRotation để vật lý không bị giật
            // Slerp để xoay mượt (0.1f là độ mượt)
            Quaternion smoothedRot = Quaternion.Slerp(transform.rotation, targetRot, 10f * Time.fixedDeltaTime);
        
            // Xoay ngang theo chuột (Xoay quanh trục Up của nhân vật hiện tại)
            smoothedRot *= Quaternion.Euler(0, mouseX * sensitivity, 0);

            _rb.MoveRotation(smoothedRot);

            // 3. Di chuyển (Movement)
            Vector2 moveInput = _inputHandler.GetMoveInput();
            bool isRunning = _inputHandler.IsRunPressed();
        
            // Lấy vector Up hiện tại của nhân vật (ngược chiều trọng lực)
            Vector3 gravityUp = -gravityForce.normalized;

            // Gọi Logic tính toán vận tốc
            Vector3 moveVelocity = _moveLogic.CalculateMoveVelocity(moveInput, _headTransform, gravityUp, isRunning);

            // Áp dụng di chuyển (Dùng MovePosition để giữ nguyên vị trí Y tương đối)
            _rb.MovePosition(_rb.position + moveVelocity * Time.fixedDeltaTime);

            // 4. Nhảy (Jump)
            if (_inputHandler.IsJumpPressed())
            {
                // Logic nhảy đơn giản: Thêm lực ngược hướng trọng lực
                // Cần thêm check IsGrounded (Dùng Raycast xuống chân) ở đây để tránh nhảy liên tục trên trời
                // Tạm thời cho nhảy thoải mái để test
                float jumpForce = _moveLogic.GetJumpForce();
                _rb.AddForce(gravityUp * jumpForce, ForceMode.Impulse);
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