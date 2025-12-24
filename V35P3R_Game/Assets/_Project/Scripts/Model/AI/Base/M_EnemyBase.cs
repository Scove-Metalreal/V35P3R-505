using _Project.Scripts.View;
using UnityEngine;
using UnityEngine.AI;

namespace _Project.Scripts.Model.AI.Base
{
    [RequireComponent(typeof(NavMeshAgent))]
    public abstract class M_EnemyBase : MonoBehaviour
    {
        [Header("--- BASE SETTINGS ---")]
        public float patrolSpeed = 3f;
        public float chaseSpeed = 6f;
        public float attackRange = 1.5f;
        public float viewDistance = 15f;
        public float viewAngle = 90f;

        [Header("--- WIRING ---")]
        public V_EnemyVisual visual; // Script Animation chung
        public Transform eyePos;

        // --- SYSTEM COMPONENTS ---
        [HideInInspector] public NavMeshAgent agent;
        [HideInInspector] public Transform targetPlayer; // Player đang bị nhắm tới
        [HideInInspector] public Vector3 lastKnownPosition; // Vị trí cuối cùng thấy player

        // --- STATE MACHINE ---
        protected EnemyState currentState;

        protected virtual void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
        }

        protected virtual void Update()
        {
            // Chạy logic của State hiện tại
            if (currentState != null)
            {
                currentState.Execute();
            }

            // Cập nhật Animation
            if (visual != null)
            {
                visual.UpdateSpeed(agent.velocity.magnitude);
            }
        }

        // Hàm đổi trạng thái (State Machine)
        public void ChangeState(EnemyState newState)
        {
            if (currentState != null)
            {
                currentState.Exit();
            }

            currentState = newState;
            currentState.Enter();
        }

        // --- CÁC HÀM CẢM QUAN (DÙNG CHUNG) ---
    
        // 1. Nhìn (Dành cho Stalker)
        public bool CanSeePlayer()
        {
            // (Logic Raycast nhìn thấy Player - copy từ C_AILogic cũ vào đây hoặc gọi C_AILogic)
            // Code demo nhanh:
            if (targetPlayer == null) FindTarget();
            if (targetPlayer == null) return false;

            Vector3 dir = (targetPlayer.position - eyePos.position).normalized;
            float dist = Vector3.Distance(eyePos.position, targetPlayer.position);
        
            if (dist < viewDistance && Vector3.Angle(eyePos.forward, dir) < viewAngle / 2)
            {
                if (!Physics.Raycast(eyePos.position, dir, dist, LayerMask.GetMask("Default"))) // Check tường
                {
                    lastKnownPosition = targetPlayer.position;
                    return true;
                }
            }
            return false;
        }

        // 2. Nghe (Dành cho con Mù)
        public bool CanHearPlayer(float hearingRange)
        {
            if (targetPlayer == null) FindTarget();
            if (targetPlayer == null) return false;

            float dist = Vector3.Distance(transform.position, targetPlayer.position);
        
            // Kiểm tra xem Player có đang chạy hoặc đi không
            // Giả sử lấy velocity của player hoặc check state player
            bool playerMakingNoise = targetPlayer.GetComponent<Rigidbody>().linearVelocity.magnitude > 2f; 

            if (dist < hearingRange && playerMakingNoise)
            {
                lastKnownPosition = targetPlayer.position;
                return true;
            }
            return false;
        }

        private void FindTarget()
        {
            // Tìm player (Tạm thời find tag)
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p) targetPlayer = p.transform;
        }
    }
}