using System;
using Unity.Netcode;
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;
public class M_CameraNetWork : NetworkBehaviour
{
    public GameObject playerCamera;
    
    [SerializeField] private CinemachineCamera cinemachineCamera; 
    
    

    public float normalPOV = 60f;
    public float zoomPOV = 90f;
    public float speedPOV = 20f;
    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            playerCamera.SetActive(false);
        }
        else
        {
            playerCamera.SetActive(true);
        }
    }

    private void Update()
    {
        if (cinemachineCamera == null)
        {
            cinemachineCamera = FindObjectOfType<CinemachineCamera>();
        }
        if (Keyboard.current.leftShiftKey.isPressed)
        {
            SetLenPOV(zoomPOV);
            
        }
        else
        {
            SetLenPOV(normalPOV);
            
        }
    }
    void SetLenPOV(float target)
    {
        var len = cinemachineCamera.Lens;
        len.FieldOfView = Mathf.Lerp(len.FieldOfView, target, speedPOV * Time.deltaTime);
        cinemachineCamera.Lens = len;
    }
}
