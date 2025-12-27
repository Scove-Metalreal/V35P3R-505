using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.Controller
{
    public class C_InventoryLogic : MonoBehaviour
    {
        [Header("--- INVENTORY SETTINGS ---")]
        [SerializeField] private int _maxSlots = 4;

        // Hàm 1: Kiểm tra xem có đủ chỗ nhặt món mới không
        public bool CanPickupItem(List<Item_Scrap> currentInventory, Item_Scrap newItem)
        {
            int currentUsed = CalculateTotalSlots(currentInventory);
        
            // Nếu số ô hiện tại + số ô món mới <= Max thì cho nhặt
            return (currentUsed + newItem.GetSlotSize()) <= _maxSlots;
        }

        // Hàm 2: Tính tổng số ô đang dùng
        public int CalculateTotalSlots(List<Item_Scrap> inventory)
        {
            int total = 0;
            foreach (var item in inventory)
            {
                if (item != null)
                    total += item.GetSlotSize();
            }
            return total;
        }

        // Hàm 3: Hỗ trợ chuyển đổi item trên tay (Ẩn món cũ, hiện món mới)
        public void RefreshHandVisuals(List<Item_Scrap> inventory, int activeIndex, Transform handPos)
        {
            for (int i = 0; i < inventory.Count; i++)
            {
                if (inventory[i] == null) continue;

                if (i == activeIndex)
                {
                    // Món đang chọn: Hiện lên + Gắn vào tay
                    inventory[i].gameObject.SetActive(true);
                    inventory[i].transform.SetParent(handPos);
                    
                    inventory[i].transform.localPosition = inventory[i].GetHoldOffset();
                    inventory[i].transform.localRotation = inventory[i].GetHoldRotation();
                }
                else
                {
                    // Món không chọn: Ẩn đi (nhét vào túi ảo)
                    inventory[i].gameObject.SetActive(false);
                }
            }
        }
    }
}