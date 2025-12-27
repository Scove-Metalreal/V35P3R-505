using System.Collections.Generic;
using _Project.Scripts.Controller;
using _Project.Scripts.Interfaces;
using _Project.Scripts.Managers;
// using _Project.Scripts.Managers;
using _Project.Scripts.View;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Project.Scripts.Model
{
    [RequireComponent(typeof(Rigidbody))]
    public class M_Player : MonoBehaviour
    {
        [Header("--- COMPONENTS (WIRING) ---")]
        [SerializeField] private C_InputHandler _inputHandler;
        [SerializeField] private C_GravityLogic _gravityLogic;
        [SerializeField] private C_MovementLogic _moveLogic;
        [SerializeField] private C_InventoryLogic _inventoryLogic;
        [SerializeField] private C_PlayerAudio _audioLogic;

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
        // Biến lưu vật đang nhìn thấy (để hiện UI)
        private IInteractable _currentTarget = null; 
        private bool _isDead = false;
        // Chỉ s món đồ đang cầm trên tay (0, 1, 2...)
        private int _currentSlotIndex = 0;
        // Danh sách đồ trong túi
        [SerializeField] private List<Item_Scrap> _inventory = new List<Item_Scrap>();
        [SerializeField] private float _currentHealth;
        [SerializeField] private float _currentOxygen;
        


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
            if (_audioLogic == null) _audioLogic = GetComponentInChildren<C_PlayerAudio>();
        
            if (_audioLogic != null)
            {
                _audioLogic.Setup(transform);
            }
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
            HandleInteraction();
            HandleCameraLook();
            HandleSurvivalStats();
            HandleInventoryInput();
            HandleAudio();
            HandleDrop();
            // 3. Visual Animation
            if (_visual != null)
            {
                _visual.UpdateMovementAnim(
                    _inputHandler.GetMoveInput(), 
                    _inputHandler.IsRunPressed(), 
                    _inputHandler.IsCrouchPressed()
                );
            }
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                // _rb.AddForce(gravityUp * _moveLogic.GetJumpForce(), ForceMode.Impulse);
                Vector3 jumpDir = -_gravityLogic.CurrentGravity.normalized;
                GetComponent<Rigidbody>().AddForce(jumpDir * _moveLogic.GetJumpForce(), ForceMode.Impulse);
            }
        }

        // --- PHYSICS LOOP: XỬ LÝ DI CHUYỂN & VẬT LÝ ---
        private void FixedUpdate()
        {
            if (_isDead) return;
            Vector3 gravityForce = _gravityLogic.CalculateGravityForce(_currentGravityDir);
            _rb.AddForce(gravityForce, ForceMode.Acceleration);
            // --- BƯỚC 1: TRỌNG LỰC ---
            
            Vector3 gravityUp = -_gravityLogic.CurrentGravity.normalized;
            

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
            if (Keyboard.current.eKey.wasPressedThisFrame && _currentTarget != null)
            {
                // Nếu tay đang rảnh thì mới nhặt
                // if (_heldItem == null)
                {
                    _currentTarget.OnInteract(this);
                }
            }
        }
        
        private void HandleCameraLook()
        {
            // Lấy Mouse Y (Lên xuống)
            
            float mouseX = Mouse.current.delta.x.ReadValue() * _moveLogic.GetSensitivity()  * Time.deltaTime;
            float mouseY = Mouse.current.delta.y.ReadValue() * _moveLogic.GetSensitivity()  * Time.deltaTime;
            // Tính toán góc gật đầu
            Quaternion headRot = _moveLogic.CalculateHeadRotation(mouseY,mouseX);
        
            // Áp dụng vào Camera
            _headTransform.localRotation = headRot;
        }
        
        public void TryPickupItem(Item_Scrap item)
        {
            // 1. Check xem đủ chỗ không
            if (!_inventoryLogic.CanPickupItem(_inventory, item))
            {
                Debug.Log("❌ Túi đầy! Không nhặt được.");
                // TODO: Hiện UI báo "Inventory Full"
                return;
            }

            // 2. Thêm vào List
            _inventory.Add(item);
        
            // 3. Tắt vật lý của item
            item.OnPickUp();

            // 4. Tự động chuyển tay sang món vừa nhặt
            _currentSlotIndex = _inventory.Count - 1;
            UpdateHandVisuals();

            Debug.Log($"Đã nhặt: {item.name}. Tổng Slot: {_inventoryLogic.CalculateTotalSlots(_inventory)}/4");
        }
        
        private void HandleDrop()
        {
            // Giả sử bấm G để vứt (cần map phím Drop trong Input System)
            // Hoặc dùng tạm nút Interact nếu đang cầm đồ (như logic cũ)
            // Ở đây tôi giả định bạn thêm nút Drop vào Input Handler
            // if (_inputHandler.IsDropPressed()) ...
        
            // Code tạm: Bấm G (dùng Input.GetKeyDown tạm để test, bạn hãy chuyển vào InputSystem sau)
            if (Keyboard.current.gKey.wasPressedThisFrame) 
            {
                DropCurrentItem();
            }
        }
        
        // 1. Trả về món đồ đang cầm trên tay (để Trạm kiểm tra)
        public Item_Scrap GetCurrentHeldItem()
        {
            if (_inventory.Count == 0) return null;
            if (_currentSlotIndex >= _inventory.Count) return null;

            return _inventory[_currentSlotIndex];
        }
        
        // 2. Xóa món đồ đang cầm (Sau khi Trạm đã nuốt)
        public void RemoveCurrentItem()
        {
            if (_inventory.Count == 0) return;

            // Không cần vứt vật lý (Drop), chỉ cần xóa khỏi List Logic
            _inventory.RemoveAt(_currentSlotIndex);

            // Reset index nếu bị lệch
            if (_currentSlotIndex >= _inventory.Count) _currentSlotIndex = _inventory.Count - 1;
            if (_currentSlotIndex < 0) _currentSlotIndex = 0;

            // Cập nhật lại hình ảnh trên tay
            _inventoryLogic.RefreshHandVisuals(_inventory, _currentSlotIndex, _holdPosition);
        }
        
        public void DropCurrentItem()
        {
            if (_inventory.Count == 0) return;
            if (_currentSlotIndex >= _inventory.Count) return;

            // Lấy item đang cầm
            Item_Scrap itemToDrop = _inventory[_currentSlotIndex];

            // 1. Xử lý vật lý vứt ra
            Vector3 throwForce = _headTransform.forward * 5f;
            itemToDrop.OnDrop(throwForce);

            // 2. Xóa khỏi List
            _inventory.RemoveAt(_currentSlotIndex);

            // 3. Cập nhật lại chỉ số (để không bị out of range)
            if (_currentSlotIndex >= _inventory.Count)
            {
                _currentSlotIndex = _inventory.Count - 1;
            }
        
            if (_currentSlotIndex < 0) _currentSlotIndex = 0;

            // 4. Cập nhật hình ảnh trên tay
            UpdateHandVisuals();
        }
        public void ConsumeCurrentItem()
        {
            if (_inventory.Count == 0) return;
            if (_currentSlotIndex >= _inventory.Count) return;

            Item_Scrap item = _inventory[_currentSlotIndex];

            Destroy(item.gameObject); // tiêu hao

            _inventory.RemoveAt(_currentSlotIndex);

            if (_currentSlotIndex >= _inventory.Count)
                _currentSlotIndex = _inventory.Count - 1;

            if (_currentSlotIndex < 0)
                _currentSlotIndex = 0;

            UpdateHandVisuals();
        }

        private void HandleInventoryInput()
        {
            if (_inventory.Count <= 1) return; // Có 0 hoặc 1 món thì không cần đổi

            // Lấy lăn chuột (Mouse Scroll)
            // Lưu ý: Input System trả về Vector2, y là lăn lên/xuống
            float scroll = UnityEngine.Input.mouseScrollDelta.y; // Dùng tạm Input cũ cho nhanh, hoặc map vào Input System

            if (scroll > 0)
            {
                _currentSlotIndex++;
                if (_currentSlotIndex >= _inventory.Count) _currentSlotIndex = 0;
                UpdateHandVisuals();
            }
            else if (scroll < 0)
            {
                _currentSlotIndex--;
                if (_currentSlotIndex < 0) _currentSlotIndex = _inventory.Count - 1;
                UpdateHandVisuals();
            }
        }
        
        private void UpdateHandVisuals()
        {
            _inventoryLogic.RefreshHandVisuals(_inventory, _currentSlotIndex, _holdPosition);
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
        
        private void HandleAudio()
        {
            if (_audioLogic == null) return;

            // Check xem có đang di chuyển không
            Vector2 input = _inputHandler.GetMoveInput();
            bool isMoving = input != Vector2.zero; // Hoặc check _rb.velocity.magnitude > 0.1f
            bool isRunning = _inputHandler.IsRunPressed();

            // Chỉ phát tiếng khi đang đứng dưới đất
            // (Bạn cần thêm biến _isGrounded vào logic check, tạm thời dùng check input)
            // Tốt nhất là check IsGrounded từ C_MovementLogic hoặc Raycast
        
            _audioLogic.ProcessFootsteps(isMoving, isRunning);
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
            
            // Gọi Manager báo thua
            Mgr_GameLevel.Instance.TriggerGameOver("Bạn đã tử nạn!");
        }
    }
}