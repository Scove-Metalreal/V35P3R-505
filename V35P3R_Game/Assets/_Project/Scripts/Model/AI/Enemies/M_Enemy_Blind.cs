using _Project.Scripts.Model.AI.Base;
using _Project.Scripts.Model.AI.States;

namespace _Project.Scripts.Model.AI.Enemies
{
    public class M_Enemy_Blind : M_EnemyBase
    {
        private State_Patrol patrolState;
        // Ta có thể viết thêm State_ChaseAudio riêng, hoặc tái sử dụng ChaseVisual nhưng logic khác
        // Ở đây tôi giả sử dùng chung State ChaseVisual nhưng logic chuyển state khác

        protected override void Awake()
        {
            base.Awake();
            patrolState = new State_Patrol(this);
        }

        private void Start()
        {
            viewDistance = 0; // Con này bị mù
            ChangeState(patrolState);
        }

        protected override void Update()
        {
            base.Update();

            // --- LOGIC RIÊNG CỦA CON MÙ ---
        
            // Nếu NGHE THẤY tiếng -> Lao tới vị trí đó
            if (currentState == patrolState)
            {
                // Con này thính tai hơn (Range 20m)
                if (CanHearPlayer(20f)) 
                {
                    agent.SetDestination(lastKnownPosition); // Chạy đến nơi phát ra tiếng
                    // Có thể tạo State_Investigate (đến nơi ngó nghiêng rồi đi tiếp)
                }
            }
        }
    }
}