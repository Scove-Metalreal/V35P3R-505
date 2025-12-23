using _Project.Scripts.Model;

namespace _Project.Scripts.Interfaces
{
    public interface IInteractable
    {
        // Hàm được gọi khi Player bấm E
        void OnInteract(M_Player player);

        // Hàm trả về dòng chữ hiển thị lên UI (VD: "Pick up Scrap", "Open Door")
        string GetInteractionPrompt();
    
        // Hàm kiểm tra xem vật này có cầm được không
        bool IsHoldable();
    }
}