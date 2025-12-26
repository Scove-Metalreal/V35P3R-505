using UnityEngine;

namespace _Project.Scripts.View
{
    public class V_EnemyVisual : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
    
        // Hash ID cho tối ưu
        private int _speedID;
        private int _attackID;

        private void Awake()
        {
            _speedID = Animator.StringToHash("Speed");
            _attackID = Animator.StringToHash("Attack");
        }

        // Cập nhật tốc độ (để Blend từ đứng yên -> đi -> chạy)
        public void UpdateSpeed(float speed)
        {
            _animator.SetFloat(_speedID, speed);
        }

        public void TriggerAttack()
        {
            _animator.SetTrigger(_attackID);
        }
    }
}