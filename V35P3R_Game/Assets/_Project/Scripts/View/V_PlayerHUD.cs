using TMPro;
using UnityEngine;
using UnityEngine.UI;
// Cần cái này để dùng Slider

// Cần cái này nếu dùng TextMeshPro

namespace _Project.Scripts.View
{
    public class V_PlayerHUD : MonoBehaviour
    {
        [Header("--- BARS ---")]
        [SerializeField] private Slider _healthSlider;
        [SerializeField] private Slider _oxygenSlider;

        [Header("--- TEXT ---")]
        [SerializeField] private TextMeshProUGUI _interactionText; // Dòng chữ "Press E to pick up"
        
        [Header("--- GAME OBJECTIVE ---")]
        [SerializeField] private Slider _progressSlider; // Tạo thêm 1 

        // Hàm cập nhật thanh Máu (Nhận vào 0 -> 1)
        public void UpdateHealth(float current, float max)
        {
            if (_healthSlider != null)
            {
                _healthSlider.value = current / max;
            }
        }

        // Hàm cập nhật thanh Oxi
        public void UpdateOxygen(float current, float max)
        {
            if (_oxygenSlider != null)
            {
                _oxygenSlider.value = current / max;
            }
        }

        // Hàm hiện/ẩn dòng chữ tương tác
        public void SetInteractionText(string text)
        {
            if (_interactionText != null)
            {
                _interactionText.text = text;
                _interactionText.gameObject.SetActive(!string.IsNullOrEmpty(text));
            }
        }
    
        // Optional: Hiệu ứng màn hình đỏ khi sắp chết
        public void ShowDamageEffect(bool isCritical)
        {
            // Code đổi màu panel đỏ ở đây (nếu có)
        }
        
        public void UpdateStationProgress(float percent)
        {
            if (_progressSlider != null) _progressSlider.value = percent;
        }
    }
}