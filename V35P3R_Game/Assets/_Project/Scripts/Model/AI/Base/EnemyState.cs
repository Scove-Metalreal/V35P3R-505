

// Abstract Class: Không thể dùng trực tiếp, phải kế thừa
namespace _Project.Scripts.Model.AI.Base
{
    public abstract class EnemyState
    {
        protected M_EnemyBase enemy; // Tham chiếu đến con quái đang thực hiện state này
        protected float stateTimer;  // Bộ đếm giờ dùng chung

        public EnemyState(M_EnemyBase enemyBase)
        {
            this.enemy = enemyBase;
        }

        public virtual void Enter() 
        { 
            stateTimer = 0; 
        }
    
        public abstract void Execute(); // Logic chạy mỗi khung hình (Update)
    
        public virtual void Exit() { }
    }
}