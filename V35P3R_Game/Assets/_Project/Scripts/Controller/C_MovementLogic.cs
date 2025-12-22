using UnityEngine;

namespace _Project.Scripts.Controller
{
    public class C_MovementLogic
    {
        public Vector3 CalculateMoveDir(Input input, Transform transform)
        {
            Vector3 moveDir;
            return  moveDir = Vector3.zero;
        }

        public float CalculateGravity(Vector2 currentY, bool isGrounded)
        {
            float gravity;
            return 0f;
        }

        public float CalculateJump(Rigidbody force)
        {
            return 0f;
        }
    }
}