using _Project.Scripts.Interfaces;
using _Project.Scripts.Model;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Item_Scrap : MonoBehaviour, IInteractable
{
    [Header("--- DATA ---")]
    [SerializeField] private string _itemName = "Scrap";
    
    [Tooltip("Vật này chiếm bao nhiêu ô? (Tối đa 4)")]
    [Range(1, 4)]
    [SerializeField] private int _slotSize = 1; 

    [SerializeField] private float _weight = 5f; 

    [Header("--- SETTINGS ---")]
    [SerializeField] private Vector3 _holdOffset = Vector3.zero; 
    [SerializeField] private Quaternion _holdRotation = Quaternion.identity; 

    private Rigidbody _rb;
    private Collider _col;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _col = GetComponent<Collider>();
        gameObject.layer = LayerMask.NameToLayer("Interactable");
    }

    public string GetInteractionPrompt()
    {
        // Hiển thị tên + số ô chiếm dụng
        return $"Pick up {_itemName} (Size: {_slotSize})";
    }

    public bool IsHoldable() => true;

    public void OnInteract(M_Player player)
    {
        player.TryPickupItem(this); // Đổi tên hàm thành TryPickup
    }

    // --- PHYSICS TOGGLE ---
    public void OnPickUp()
    {
        _rb.isKinematic = true;
        _col.enabled = false;
        // Logic mới: Khi nhặt vào túi, nếu không cầm trên tay thì ẩn đi
        // Nhưng tạm thời cứ để logic này, M_Player sẽ quyết định ẩn/hiện
    }

    public void OnDrop(Vector3 throwForce)
    {
        _rb.isKinematic = false;
        _col.enabled = true;
        _rb.AddForce(throwForce, ForceMode.Impulse);
        
        // Bắt buộc hiện lại khi vứt ra
        gameObject.SetActive(true);
        transform.SetParent(null);
    }
    
    // GETTERS
    public int GetSlotSize() => _slotSize;
    public float GetWeight() => _weight;
    public Vector3 GetHoldOffset() => _holdOffset;
    public Quaternion GetHoldRotation() => _holdRotation;
}