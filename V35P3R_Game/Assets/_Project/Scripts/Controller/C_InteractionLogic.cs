using _Project.Scripts.Interfaces;
using UnityEngine;

namespace _Project.Scripts.Controller
{
    public class C_InteractionLogic : MonoBehaviour
    {
        [Header("--- SETTINGS ---")]
        [SerializeField] private float _interactRange = 3f;
        [SerializeField] private LayerMask _interactLayer; // Chỉ tương tác với Layer "Interactable"

        // Hàm 1: Bắn tia từ Camera để tìm vật thể
        // Trả về IInteractable nếu trúng, null nếu không trúng
        public IInteractable RaycastCheck(Transform cameraTransform)
        {
            Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        
            // Bắn Raycast
            if (Physics.Raycast(ray, out RaycastHit hit, _interactRange, _interactLayer))
            {
                // Thử lấy component IInteractable từ vật bắn trúng
                if (hit.collider.TryGetComponent(out IInteractable interactable))
                {
                    return interactable;
                }
            }
            return null;
        }

        // Hàm 2: Xử lý logic Cầm đồ (Attach)
        // Biến vật phẩm thành con của HoldPosition, tắt vật lý
        public void AttachItemToHand(Transform item, Transform handPos)
        {
            item.SetParent(handPos);
            item.localPosition = Vector3.zero; // Về đúng vị trí tay
            item.localRotation = Quaternion.identity; // Xoay theo tay
        
            // Nếu vật có Rigidbody, phải tắt nó đi để không bị rơi
            if (item.TryGetComponent(out Rigidbody rb))
            {
                rb.isKinematic = true; // Tắt vật lý
                rb.detectCollisions = false; // Tắt va chạm để không đẩy Player
            }
        }

        // Hàm 3: Xử lý logic Thả đồ (Detach)
        public void DetachItem(Transform item, Vector3 throwForce)
        {
            item.SetParent(null); // Không làm con ai nữa
        
            if (item.TryGetComponent(out Rigidbody rb))
            {
                rb.isKinematic = false; // Bật lại vật lý
                rb.detectCollisions = true;
                rb.AddForce(throwForce, ForceMode.Impulse); // Ném nhẹ ra xa
            }
        }
    }
}