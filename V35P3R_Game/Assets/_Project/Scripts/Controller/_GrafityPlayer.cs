using UnityEngine;
using UnityEngine.InputSystem;

public class _GrafityPlayer : MonoBehaviour
{
    public enum GravityDirection
    {
        Down, Up, Left, Right, Forward, Backward
    }

    [Header("Gravity Data")]
    public Vector3 Gravity = Vector3.down * 20f;
    public Vector2 GravityRotation = Vector2.zero; // x = X rot, y = Z rot

    public Vector3 CurrentGravity => Gravity;
    public float currentXRotation => GravityRotation.x;
    public float currentZRotation => GravityRotation.y;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Keyboard.current.leftCtrlKey.isPressed)
        {
            if (Keyboard.current.wKey.wasPressedThisFrame) SetGravity(GravityDirection.Up);
            if (Keyboard.current.sKey.wasPressedThisFrame) SetGravity(GravityDirection.Down);
            if (Keyboard.current.aKey.wasPressedThisFrame) SetGravity(GravityDirection.Left);
            if (Keyboard.current.dKey.wasPressedThisFrame) SetGravity(GravityDirection.Right);
            if (Keyboard.current.eKey.wasPressedThisFrame) SetGravity(GravityDirection.Forward);
            if (Keyboard.current.qKey.wasPressedThisFrame) SetGravity(GravityDirection.Backward);
        }
    }

    void FixedUpdate()
    {
        rb.AddForce(Gravity, ForceMode.Acceleration);
    }

    
    void SetGravity(GravityDirection dir)
    {
        switch (dir)
        {
            case GravityDirection.Down:
                Gravity = Vector3.down * 9.81f;
                GravityRotation = new Vector2(0f, 0f);
                break;

            case GravityDirection.Up:
                Gravity = Vector3.up * 9.81f;
                GravityRotation = new Vector2(0f, 180f);
                break;

            case GravityDirection.Left:
                Gravity = Vector3.left * 9.81f;
                GravityRotation = new Vector2(0f, -90f);
                break;

            case GravityDirection.Right:
                Gravity = Vector3.right * 9.81f;
                GravityRotation = new Vector2(0f, 90f);
                break;

            case GravityDirection.Forward:
                Gravity = Vector3.forward * 9.81f;
                GravityRotation = new Vector2(-90f, 0f);
                break;

            case GravityDirection.Backward:
                Gravity = Vector3.back * 9.81f;
                GravityRotation = new Vector2(90f, 0f);
                break;
        }
    }
}
