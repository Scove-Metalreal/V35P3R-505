using _Project.Scripts.Interfaces;
using _Project.Scripts.Managers;
using _Project.Scripts.Utilities;
using _Project.Scripts.View;
using UnityEngine;
using UnityEngine.Events;

namespace _Project.Scripts.Model.Environment
{
    public class M_RepairSite : MonoBehaviour, IInteractable
    {
        [Header("--- REQUIREMENTS ---")]
        [SerializeField] private string _siteName = "Main Generator";
        [SerializeField] private ScrapType _requiredType = ScrapType.Electronic; // Cần loại gì?
        [SerializeField] private int _requiredAmount = 3; // Cần bao nhiêu cái?

        [Header("--- STATE ---")]
        [SerializeField] private int _currentAmount = 0;
        [SerializeField] private bool _isRepaired = false;

        [Header("--- WIRING ---")]
        [SerializeField] private V_RepairSiteVisual _visual; // Kéo script Visual vào

        [Header("--- EVENTS ---")]
        public UnityEvent OnRepaired; // Kéo các sự kiện game vào đây (Mở cửa, Bật điện...)

        private void Start()
        {
            // Khởi tạo trạng thái hình ảnh ban đầu
            if (_visual != null) _visual.SetState(_isRepaired);
        }

        // --- INTERFACE ---
        public string GetInteractionPrompt()
        {
            if (_isRepaired) return $"{_siteName}: ONLINE";
        
            // Hiện thông báo: "Cần 2/3 Electronic"
            return $"{_siteName}: Need {_currentAmount}/{_requiredAmount} {_requiredType}";
        }

        public bool IsHoldable() => false;

        public void OnInteract(M_Player player)
        {
            if (_isRepaired) return;

            // 1. Kiểm tra player có cầm đúng loại đồ không
            Item_Scrap heldItem = player.GetCurrentHeldItem();

            if (heldItem == null) 
            {
                Debug.Log("Tay không thì sửa kiểu gì?");
                return;
            }

            // 2. So sánh Type
            if (heldItem.GetScrapType() == _requiredType)
            {
                // Đúng loại -> Lấy đồ
                InsertItem(heldItem, player);
            }
            else
            {
                Debug.Log($"Sai đồ rồi! Cần {_requiredType}, bạn đang cầm {heldItem.GetScrapType()}");
                // Có thể play âm thanh báo lỗi "Buzz!"
            }
        }

        // --- LOGIC HELPER ---
        private void InsertItem(Item_Scrap item, M_Player player)
        {
            // 1. Xóa khỏi tay Player
            player.RemoveCurrentItem();
            Destroy(item.gameObject); // Hủy vật thể

            // 2. Tăng tiến độ
            _currentAmount++;

            // 3. Check hoàn thành
            if (_currentAmount >= _requiredAmount)
            {
                CompleteRepair();
            }
            else
            {
                Debug.Log($"Đã nạp 1 cái. Còn thiếu {_requiredAmount - _currentAmount}");
            }
        }

        private void CompleteRepair()
        {
            _isRepaired = true;
        
            // Update Visual
            if (_visual != null)
            {
                _visual.SetState(true); // Chuyển sang trạng thái Sửa xong
                _visual.PlayRepairSuccessFX();
            }

            // Gọi Event (Ví dụ: Mở cửa, Win game...)
            OnRepaired?.Invoke();
        
            Debug.Log($"🎉 {_siteName} ĐÃ SỬA XONG!");
            Mgr_GameLevel.Instance.TriggerVictory();
        }
    }
}