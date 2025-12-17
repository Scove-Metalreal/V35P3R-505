using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class C_PlayerController : NetworkBehaviour
{
    private C_PlayerGrafity playerGrafity;
    private Rigidbody rb;
    private M_PlayerStats playerStat;

    public float sensitivity = 200f;
    bool canMove = true;

    float xRotation;
    float yRotation;

    Vector2 moveInput;

    UnityEngine.InputSystem.PlayerInput playerInput;

    private void Awake()
    {
        playerGrafity = GetComponent<C_PlayerGrafity>();
        playerStat = GetComponent<M_PlayerStats>();
        rb = GetComponent<Rigidbody>();

<<<<<<< Updated upstream
        // Input setup
        playerInput = new PlayerInput();
        playerInput.Player.Move.performed += ctx =>
        {
            if (IsOwner) moveInput = ctx.ReadValue<Vector2>();
        };
        playerInput.Player.Move.canceled += ctx =>
        {
            if (IsOwner) moveInput = Vector2.zero;
        };
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            playerInput.Disable();
            enabled = false;
            return;
        }

        playerInput.Enable();
        Cursor.lockState = CursorLockMode.Locked;
=======
        playerInput = GetComponent<UnityEngine.InputSystem.PlayerInput>();   // LẤY TỪ PREFAB – ĐÚNG
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            playerInput.enabled = true;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            playerInput.enabled = false;
        }
>>>>>>> Stashed changes
    }

    // GỌI TỪ PlayerInput → Move event
    public void OnMove(InputValue value)
    {
        if (!IsOwner) return;
        moveInput = value.Get<Vector2>();
    }

    void Update()
    {
        if (!IsOwner) return;
        HandleLook();
    }

    void FixedUpdate()
    {
        if (!IsOwner) return;
        HandleMove();
    }

    void HandleMove()
    {
        Vector3 gravityDir = playerGrafity.CurrentGravity.normalized;
        Vector3 up = -gravityDir;

        Vector3 forward = Vector3.ProjectOnPlane(transform.forward, up).normalized;
        Vector3 right = Vector3.ProjectOnPlane(transform.right, up).normalized;

        Vector3 move = forward * moveInput.y + right * moveInput.x;

        float speed = Keyboard.current.leftShiftKey.isPressed ?
            playerStat.RunSpeed.Value :
            playerStat.WalkSpeed.Value;

        rb.MovePosition(rb.position + move * speed * Time.fixedDeltaTime);
    }

    void HandleLook()
    {
        float mouseX = Mouse.current.delta.x.ReadValue() * sensitivity * Time.deltaTime;
        float mouseY = Mouse.current.delta.y.ReadValue() * sensitivity * Time.deltaTime;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        Quaternion lookRot = Quaternion.Euler(xRotation, yRotation, 0);
        Quaternion gravityRot = Quaternion.Euler(playerGrafity.currentXRotation, 0, playerGrafity.currentZRotation);

        transform.rotation = gravityRot * lookRot;
    }
}
