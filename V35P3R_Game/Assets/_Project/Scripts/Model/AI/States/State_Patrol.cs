using _Project.Scripts.Model.AI.Base;
using UnityEngine;

namespace _Project.Scripts.Model.AI.States
{
    public class State_Patrol : EnemyState
    {
        public State_Patrol(M_EnemyBase enemy) : base(enemy) { }

        public override void Enter()
        {
            enemy.agent.speed = enemy.patrolSpeed;
            enemy.agent.isStopped = false;
            SetRandomDestination();
        }

        public override void Execute()
        {
            // Đến nơi thì tìm điểm mới
            if (!enemy.agent.pathPending && enemy.agent.remainingDistance < 0.5f)
            {
                stateTimer += Time.deltaTime;
                if (stateTimer > 2f) // Nghỉ 2s
                {
                    SetRandomDestination();
                    stateTimer = 0;
                }
            }
        }

        private void SetRandomDestination()
        {
            Vector3 randomPoint = Random.insideUnitSphere * 10f + enemy.transform.position;
            UnityEngine.AI.NavMeshHit hit;
            if (UnityEngine.AI.NavMesh.SamplePosition(randomPoint, out hit, 10f, 1))
            {
                enemy.agent.SetDestination(hit.position);
            }
        }
    }
}