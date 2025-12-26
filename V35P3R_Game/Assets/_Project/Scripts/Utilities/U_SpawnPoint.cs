using UnityEngine;

namespace _Project.Scripts.Utilities
{
    public class U_SpawnPoint : MonoBehaviour
    {
        // Vẽ gizmos để dễ nhìn thấy trong Editor
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, 0.5f);
            Gizmos.DrawLine(transform.position, transform.position + Vector3.up * 2);
        }
    }
}