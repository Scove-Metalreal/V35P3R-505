using UnityEngine;

namespace _Project.Scripts.View
{
    public class V_RepairSiteVisual : MonoBehaviour
    {
        [Header("--- BROKEN STATE FX ---")]
        [SerializeField] private GameObject[] _brokenVFX; // Kéo lửa, khói, tia điện vào đây
        [SerializeField] private GameObject _brokenModel; // Model nát (nếu có)

        [Header("--- FIXED STATE FX ---")]
        [SerializeField] private GameObject[] _fixedVFX;  // Kéo đèn xanh, particle đẹp vào đây
        [SerializeField] private GameObject _fixedModel;  // Model lành lặn

        // Hàm gọi lúc Start
        public void SetState(bool isRepaired)
        {
            // Bật/Tắt FX Hỏng
            if (_brokenModel != null) _brokenModel.SetActive(!isRepaired);
            foreach (var fx in _brokenVFX) fx.SetActive(!isRepaired);

            // Bật/Tắt FX Sửa xong
            if (_fixedModel != null) _fixedModel.SetActive(isRepaired);
            foreach (var fx in _fixedVFX) fx.SetActive(isRepaired);
        }
    
        public void PlayRepairSuccessFX()
        {
            // Có thể thêm âm thanh hoặc particle nổ pháo hoa nhẹ ở đây
        }
    }
}