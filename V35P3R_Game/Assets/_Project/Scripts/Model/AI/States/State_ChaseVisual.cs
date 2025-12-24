using _Project.Scripts.Model.AI.Base;
using UnityEngine;

namespace _Project.Scripts.Model.AI.States
{
    public class State_ChaseVisual : EnemyState
    {
        public State_ChaseVisual(M_EnemyBase enemy) : base(enemy) { }

        public override void Enter()
        {
            enemy.agent.speed = enemy.chaseSpeed;
        }

        public override void Execute()
        {
            if (enemy.targetPlayer == null) return;

            // 1. Chạy tới vị trí Player
            enemy.agent.SetDestination(enemy.targetPlayer.position);

            // 2. Nếu mất dấu (không nhìn thấy nữa)
            if (!enemy.CanSeePlayer())
            {
                // Đến vị trí cuối cùng thấy rồi quay về đi tuần
                if (enemy.agent.remainingDistance < 1f)
                {
                    // Chuyển về trạng thái đi tuần (Cần logic chuyển đổi ở Enemy gốc, hoặc gọi callback)
                    // Ở đây demo đơn giản: Enemy tự quyết định
                    // Tuy nhiên, cách hay nhất là M_EnemyBase quyết định việc chuyển state
                }
            }

            // 3. Tấn công nếu đủ gần
            if (Vector3.Distance(enemy.transform.position, enemy.targetPlayer.position) < enemy.attackRange)
            {
                // Chuyển sang State Attack (chưa viết)
            }
        }
    }
}