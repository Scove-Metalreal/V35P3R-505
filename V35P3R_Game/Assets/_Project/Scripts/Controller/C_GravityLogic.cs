using UnityEngine;

// Script này chỉ chịu trách nhiệm TÍNH TOÁN hướng và lực.
// Nó KHÔNG tự ý thay đổi Transform hay Rigidbody của Player.
namespace _Project.Scripts.Controller
{
    public class C_GravityLogic : MonoBehaviour
    {
        // Định nghĩa Enum ngay tại đây để các script khác dùng chung
        public enum GravityDirection 
        {
            Down, Up, Left, Right, Forward, Backward
        }

        [Header("--- SETTINGS ---")]
        [SerializeField] private float _gravityForce = 20f; // Giá trị gốc của bạn là 20f

        // Hàm 1: Từ Enum -> Trả về Vector lực hút (để M_Player AddForce)
        public Vector3 CalculateGravityForce(GravityDirection dir)
        {
            switch (dir)
            {
                case GravityDirection.Down:     return Vector3.down * _gravityForce;
                case GravityDirection.Up:       return Vector3.up * _gravityForce;
                case GravityDirection.Left:     return Vector3.left * _gravityForce;
                case GravityDirection.Right:    return Vector3.right * _gravityForce;
                case GravityDirection.Forward:  return Vector3.forward * _gravityForce;
                case GravityDirection.Backward: return Vector3.back * _gravityForce;
                default:                        return Vector3.down * _gravityForce;
            }
        }

        // Hàm 2: Từ Enum -> Trả về Góc xoay của nhân vật (Quaternion)
        // Tôi đã tối ưu hóa logic Vector2 (x, z) cũ của bạn thành Quaternion chuẩn Unity
        public Quaternion CalculateTargetRotation(GravityDirection dir)
        {
            // Logic gốc của bạn:
            // Down: (0, 0), Up: (0, 180), Left: (0, -90)...
        
            switch (dir)
            {
                case GravityDirection.Down:     
                    return Quaternion.Euler(0f, 0f, 0f);
            
                case GravityDirection.Up:       
                    return Quaternion.Euler(0f, 0f, 180f);
            
                case GravityDirection.Left:     
                    return Quaternion.Euler(0f, 0f, -90f);
            
                case GravityDirection.Right:    
                    return Quaternion.Euler(0f, 0f, 90f);
            
                case GravityDirection.Forward:  
                    return Quaternion.Euler(-90f, 0f, 0f);
            
                case GravityDirection.Backward: 
                    return Quaternion.Euler(90f, 0f, 0f);
                
                default: return Quaternion.identity;
            }
        }

        // Hàm 3: Logic xác định hướng trọng lực mới dựa trên Input (WASD + Ctrl)
        // Trả về hướng mới, hoặc trả về hướng cũ nếu không có input hợp lệ
        public GravityDirection GetNewDirectionFromInput(GravityDirection currentDir, Vector2 inputMove, bool isInteractPressed, bool isAbilityPressed)
        {
            // Giả lập logic cũ: Ctrl + Phím -> Đổi hướng
            // Vì Input System trả về Vector2, ta map nó sang hướng
        
            // Input: W (y=1), S (y=-1), A (x=-1), D (x=1)
            if (inputMove.y > 0.5f) return GravityDirection.Up;         // W
            if (inputMove.y < -0.5f) return GravityDirection.Down;      // S
            if (inputMove.x < -0.5f) return GravityDirection.Left;      // A
            if (inputMove.x > 0.5f) return GravityDirection.Right;      // D
        
            if (isInteractPressed) return GravityDirection.Forward;     // E (Interact)
            if (isAbilityPressed) return GravityDirection.Backward;     // Q (Ability) - Giả sử bạn map Q vào Ability

            return currentDir; // Không đổi
        }
    }
}