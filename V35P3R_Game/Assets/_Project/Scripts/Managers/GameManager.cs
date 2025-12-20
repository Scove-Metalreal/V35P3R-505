using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    
    [SerializeField] private CinemachineCamera cinemachineCamera;
    public float normalPOV = 60f;
    public float zoomPOV = 90f;
    public float speedPOV = 20f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
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
