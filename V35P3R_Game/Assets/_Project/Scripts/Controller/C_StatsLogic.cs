using UnityEngine;

namespace _Project.Scripts.Controller
{
    public class C_StatsLogic : MonoBehaviour
    {
        [Header("--- HEALTH SETTINGS ---")]
        [SerializeField] private float _maxHealth = 100f;
    
        [Header("--- OXYGEN SETTINGS ---")]
        [SerializeField] private float _maxOxygen = 100f;
        [SerializeField] private float _oxygenDrainRate = 1.5f; // Mất 1.5 Oxi mỗi giây
        [SerializeField] private float _suffocationDamage = 5f; // Máu mất mỗi giây khi hết Oxi

        // --- GETTERS (Để M_Player lấy thông số khởi tạo) ---
        public float GetMaxHealth() => _maxHealth;
        public float GetMaxOxygen() => _maxOxygen;

        // --- LOGIC 1: TÍNH TOÁN OXI ---
        // Trả về lượng Oxi còn lại sau khi trừ
        public float CalculateOxygenDrain(float currentOxygen, float deltaTime)
        {
            // Trừ Oxi theo thời gian
            float newOxygen = currentOxygen - (_oxygenDrainRate * deltaTime);
        
            // Kẹp giá trị không cho âm
            return Mathf.Clamp(newOxygen, 0f, _maxOxygen);
        }

        // --- LOGIC 2: TÍNH TOÁN SÁT THƯƠNG (DAMAGE) ---
        // Trả về lượng Máu còn lại
        public float CalculateHealthChange(float currentHealth, float changeAmount)
        {
            // changeAmount âm là trừ máu, dương là hồi máu
            float newHealth = currentHealth + changeAmount;
        
            return Mathf.Clamp(newHealth, 0f, _maxHealth);
        }

        // --- LOGIC 3: CHECK CHẾT ---
        public bool IsDead(float currentHealth)
        {
            return currentHealth <= 0f;
        }

        // Logic phụ: Nếu hết Oxi thì trả về lượng damage cần trừ vào máu
        public float GetSuffocationDamage(float currentOxygen)
        {
            if (currentOxygen <= 0f)
            {
                return -_suffocationDamage; // Trả về số âm để trừ máu
            }
            return 0f;
        }
    }
}