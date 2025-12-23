using _Project.Scripts.Interfaces;
using _Project.Scripts.Model;
using UnityEngine;

namespace _Project.Scripts.Managers
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class Item_Scrap : MonoBehaviour, IInteractable
    {
        [Header("--- DATA ---")]
        [SerializeField] private string _itemName = "Rusty Gear";
        [SerializeField] private float _weight = 5f; // Trọng lượng làm chậm player
        [SerializeField] private int _value = 10;    // Giá tiền

        public string GetInteractionPrompt()
        {
            return $"Pick up {_itemName} (${_value})";
        }

        public bool IsHoldable() => true;

        // Logic khi bị bấm E
        public void OnInteract(M_Player player)
        {
            // Gọi hàm Pickup của Player để Player xử lý việc cầm
            player.PickupItem(this);
        }
    
        // Getter cho Player biết item này nặng bao nhiêu
        public float GetWeight() => _weight;
    }
}