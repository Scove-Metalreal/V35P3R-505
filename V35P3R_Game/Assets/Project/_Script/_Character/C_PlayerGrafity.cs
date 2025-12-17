using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class C_PlayerGrafity : NetworkBehaviour
{
    public enum GravityDirection
    {
        Down, Up, Left, Right, Forward, Backward
    }

    public NetworkVariable<Vector3> Gravity =
        new NetworkVariable<Vector3>(Vector3.down,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server);

    public NetworkVariable<Vector2> GravityRotation =
        new NetworkVariable<Vector2>(Vector2.zero,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server);


    public Vector3 CurrentGravity => Gravity.Value;
    public float currentXRotation => GravityRotation.Value.x;
    public float currentZRotation => GravityRotation.Value.y;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!IsOwner) return;

        if (Keyboard.current.leftCtrlKey.isPressed)
        {
            if (Keyboard.current.wKey.wasPressedThisFrame) SetGravityServerRpc(GravityDirection.Up);
            if (Keyboard.current.sKey.wasPressedThisFrame) SetGravityServerRpc(GravityDirection.Down);
            if (Keyboard.current.aKey.wasPressedThisFrame) SetGravityServerRpc(GravityDirection.Left);
            if (Keyboard.current.dKey.wasPressedThisFrame) SetGravityServerRpc(GravityDirection.Right);
            if (Keyboard.current.eKey.wasPressedThisFrame) SetGravityServerRpc(GravityDirection.Forward);
            if (Keyboard.current.qKey.wasPressedThisFrame) SetGravityServerRpc(GravityDirection.Backward);
        }
    }

    [ServerRpc]
    void SetGravityServerRpc(GravityDirection dir)
    {
        switch (dir)
        {
            case GravityDirection.Down:
                Gravity.Value = Vector3.down * 20f;
                GravityRotation.Value = new Vector2(0f, 0f);
                break;

            case GravityDirection.Up:
                Gravity.Value = Vector3.up * 20f;
                GravityRotation.Value = new Vector2(0f, 180f);
                break;

            case GravityDirection.Left:
                Gravity.Value = Vector3.left * 20f;
                GravityRotation.Value = new Vector2(0f, -90f);
                break;

            case GravityDirection.Right:
                Gravity.Value = Vector3.right * 20f;
                GravityRotation.Value = new Vector2(0f, 90f);
                break;

            case GravityDirection.Forward:
                Gravity.Value = Vector3.forward * 20f;
                GravityRotation.Value = new Vector2(-90f, 0f);
                break;

            case GravityDirection.Backward:
                Gravity.Value = Vector3.back * 20f;
                GravityRotation.Value = new Vector2(90f, 0f);
                break;
        }
    }

    void FixedUpdate()
    {
        rb.AddForce(Gravity.Value, ForceMode.Acceleration);
    }
}
