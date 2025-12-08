using UnityEngine;
using UnityEngine.InputSystem;

public class C_PlayerGrafity : MonoBehaviour
{
    public enum GravityDirection
    {
        Up,
        Down,
        Left,
        Right,
        Forward,
        Backward
    }

    public bool GrafityDown = true;
    public bool GrafityUp = false;
    public bool GrafityLeft = false;
    public bool GrafityRight = false;
public bool GrafityForward = false;
public bool GrafityBackward = false;
    
    public bool isGround = true;
    private bool isChangingGravity = false;
    public float currentZRotation = 0f;
    public float currentXRotation = 0f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
    }

    void Update()
    {
        // Đổi trọng lực bằng Ctrl + WASD
        if (Keyboard.current.leftCtrlKey.isPressed)   
        {
            if (Keyboard.current.wKey.wasPressedThisFrame)
                ApplyGravityDirection(GravityDirection.Up);

            if (Keyboard.current.sKey.wasPressedThisFrame)
                ApplyGravityDirection(GravityDirection.Down);

            if (Keyboard.current.aKey.wasPressedThisFrame)
                ApplyGravityDirection(GravityDirection.Left);

            if (Keyboard.current.dKey.wasPressedThisFrame)
                ApplyGravityDirection(GravityDirection.Right);

            if (Keyboard.current.eKey.wasPressedThisFrame)
                ApplyGravityDirection(GravityDirection.Forward);

            if (Keyboard.current.qKey.wasPressedThisFrame)
                ApplyGravityDirection(GravityDirection.Backward);
        }
        if (isChangingGravity)
        {
            if (isGround)
            {
                isChangingGravity = false;
                Physics.gravity = Vector3.down * 9.81f;  
            }
            return;
        }
    }

    public void StartGravityChange()
    {
        isChangingGravity = true;
        isGround = false;

        rb.linearVelocity = Vector3.zero;

        Physics.gravity *= 3f; // tăng trọng lực tạm thời để hút player
    }

    public void ApplyGravityDirection(GravityDirection direction)
    {
        switch (direction)
        {
            case GravityDirection.Forward:
                Physics.gravity = Vector3.forward * 9.81f;
                currentZRotation = 0f;
                currentXRotation = -90f;
                GrafityForward = true; GrafityDown = GrafityLeft = GrafityRight = GrafityBackward= GrafityUp = false;
                break;
            case GravityDirection.Backward:
                Physics.gravity = Vector3.back * 9.81f;
                currentZRotation = 0f;
                currentXRotation = 90f;
                GrafityBackward = true; GrafityDown = GrafityLeft = GrafityRight = GrafityForward= GrafityUp = false;
                break;
            case GravityDirection.Up:
                Physics.gravity = Vector3.up * 9.81f;
                currentZRotation = 180f;
                currentXRotation = 0f;
                GrafityUp = true; GrafityDown = GrafityLeft = GrafityRight = GrafityForward = GrafityBackward = false;
                break;

            case GravityDirection.Left:
                Physics.gravity = Vector3.left * 9.81f;
                currentZRotation = -90f;
                currentXRotation = 0f;
                GrafityLeft = true; GrafityDown = GrafityUp = GrafityRight= GrafityForward = GrafityBackward = false;
                break;

            case GravityDirection.Right:
                Physics.gravity = Vector3.right * 9.81f;
                currentZRotation = 90f;
                currentXRotation = 0f;
                GrafityRight = true; GrafityDown = GrafityUp = GrafityLeft= GrafityForward = GrafityBackward = false;
                break;

            case GravityDirection.Down:
            default:
                Physics.gravity = Vector3.down * 9.81f;
                currentZRotation = 0f;
                currentXRotation = 0f;
                GrafityDown = true; GrafityUp = GrafityLeft = GrafityRight= GrafityForward = GrafityBackward = false;
                break;
        }

        StartGravityChange();
    }
}
