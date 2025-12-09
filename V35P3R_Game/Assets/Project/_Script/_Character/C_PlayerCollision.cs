using System;
using UnityEngine;

public class C_PlayerCollision : MonoBehaviour
{
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            
        }
    }
}
