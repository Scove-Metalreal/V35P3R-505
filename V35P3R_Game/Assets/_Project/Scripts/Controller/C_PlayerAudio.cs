using _Project.Scripts.Managers;
using UnityEngine;

namespace _Project.Scripts.Controller
{
    public class C_PlayerAudio : MonoBehaviour
    {
        [Header("--- SETTINGS ---")]
        [SerializeField] private float _stepIntervalWalk = 0.5f; // Đi bộ: 0.5s/bước
        [SerializeField] private float _stepIntervalRun = 0.3f;  // Chạy: 0.3s/bước
        [SerializeField] private LayerMask _groundLayer;

        private float _stepTimer = 0f;
        private Transform _playerTransform; // Tham chiếu đến chân Player

        public void Setup(Transform playerTransform)
        {
            _playerTransform = playerTransform;
        }

        // Hàm được gọi từ M_Player mỗi khung hình
        public void ProcessFootsteps(bool isMoving, bool isRunning)
        {
            if (!isMoving) 
            {
                _stepTimer = 0f; // Reset để bước đầu tiên phát ngay lập tức
                return;
            }

            // Đếm giờ
            _stepTimer += Time.deltaTime;

            float interval = isRunning ? _stepIntervalRun : _stepIntervalWalk;

            if (_stepTimer >= interval)
            {
                PlayStepSound();
                _stepTimer = 0f;
            }
        }

        private void PlayStepSound()
        {
            // 1. Bắn Raycast xuống đất xem đang đứng trên cái gì
            if (Physics.Raycast(_playerTransform.position + Vector3.up * 0.5f, -_playerTransform.up, out RaycastHit hit, 1.5f, _groundLayer))
            {
                AudioClip clipToPlay = null;

                // 2. Check Tag của sàn nhà
                if (hit.collider.CompareTag("Metal"))
                {
                    clipToPlay = Mgr_AudioManager.Instance.Config.GetRandomClip(Mgr_AudioManager.Instance.Config.stepsMetal);
                }
                else // Mặc định là bê tông/đất
                {
                    clipToPlay = Mgr_AudioManager.Instance.Config.GetRandomClip(Mgr_AudioManager.Instance.Config.stepsConcrete);
                }

                // 3. Gọi Manager phát tiếng tại chân
                if (clipToPlay != null)
                {
                    // Random nhẹ pitch để tiếng đỡ chán
                    Mgr_AudioManager.Instance.PlaySFX_3D(clipToPlay, hit.point, 0.8f);
                }
            }
        }
    }
}