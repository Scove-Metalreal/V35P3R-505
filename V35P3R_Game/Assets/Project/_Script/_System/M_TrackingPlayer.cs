using UnityEngine;
using Unity.Cinemachine;

public class M_TrackingPlayer : MonoBehaviour
{
    private CinemachineCamera cinemachineCamera;
    private Transform playerTransform;

   

    private void Awake()
    {
        cinemachineCamera = GetComponent<CinemachineCamera>();

        

    }

    private void FixedUpdate()
    {
        playerTransform = GameObject.FindWithTag("Player")?.transform;
        if (cinemachineCamera.Follow == null)
        {
            cinemachineCamera.Follow = playerTransform;
        }
    }
}