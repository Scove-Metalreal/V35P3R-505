using UnityEngine;

public class C_PlayerCollision : MonoBehaviour
{
    [Header("Ground Check")]
        public bool isGrounded;
        public string groundTag = "Ground";
    
        private void OnCollisionEnter(Collision collision)
        {
            // Chạm đất
            if (collision.gameObject.CompareTag(groundTag))
            {
                isGrounded = true;
              
            }
            
        }
    
        private void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.CompareTag(groundTag))
            {
                isGrounded = false;
               
            }
        }
    
}
