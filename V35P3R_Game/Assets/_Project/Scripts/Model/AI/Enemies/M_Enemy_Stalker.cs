using _Project.Scripts.Model.AI.Base;
using _Project.Scripts.Model.AI.States;
using UnityEngine;

namespace _Project.Scripts.Model.AI.Enemies
{
    public class M_Enemy_Stalker : M_EnemyBase
    {
        // Khai báo các State mà con này có
        private State_Patrol patrolState;
        private State_ChaseVisual chaseState;

        protected override void Awake()
        {
            base.Awake();
            // Khởi tạo các viên gạch
            patrolState = new State_Patrol(this);
            chaseState = new State_ChaseVisual(this);
        }

        private void Start()
        {
            ChangeState(patrolState); // Bắt đầu bằng đi tuần
        }

        protected override void Update()
        {
            base.Update(); // Chạy logic của State hiện tại

            // --- ĐIỀU KIỆN CHUYỂN STATE RIÊNG CỦA STALKER ---
        
            // Nếu đang đi tuần mà NHÌN THẤY player -> Đuổi
            if (currentState == patrolState)
            {
                if (CanSeePlayer())
                {
                    ChangeState(chaseState);
                }
            }
        
            // Nếu đang đuổi mà MẤT DẤU -> Quay lại đi tuần
            if (currentState == chaseState)
            {
                if (!CanSeePlayer() && Vector3.Distance(transform.position, lastKnownPosition) < 1f)
                {
                    ChangeState(patrolState);
                }
            }
        }
    }
}