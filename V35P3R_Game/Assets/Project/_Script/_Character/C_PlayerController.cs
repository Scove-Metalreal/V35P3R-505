using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class C_PLayerController : NetworkBehaviour
{
    private C_PlayerGrafity playerGrafity;
    private Rigidbody rb;
    private M_PlayerStats playerStat;

    public bool canMove = true;
    public float sensitivity = 200f;

    private float xRotation = 0f;
    private float yRotation = 0f;

    PlayerInput playerInput;
    Vector2 moveInput;

    private void Awake()
    {
        playerStat = GetComponent<M_PlayerStats>();
        playerGrafity = GetComponent<C_PlayerGrafity>();

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
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        canMove = true;
    }

    void Update()
    {
        if (!IsOwner) return;
        RotationMouse();
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;
        if (canMove) PlayerMove();
    }

    void PlayerMove()
    {
        // Gravity riêng
        Vector3 gravityDir = playerGrafity.CurrentGravity.normalized;
        Vector3 up = -gravityDir;

        // Hướng di chuyển theo gravity cá nhân
        Vector3 forward = Vector3.ProjectOnPlane(transform.forward, up).normalized;
        Vector3 right = Vector3.ProjectOnPlane(transform.right, up).normalized;

        Vector3 move = forward * moveInput.y + right * moveInput.x;

        float speed = Keyboard.current.leftShiftKey.isPressed ?
                      playerStat.RunSpeed.Value :
                      playerStat.WalkSpeed.Value;

        rb.MovePosition(rb.position + move * speed * Time.fixedDeltaTime);

        PlayerJump();
    }

    void PlayerJump()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Vector3 jumpDir = -playerGrafity.CurrentGravity.normalized;
            rb.AddForce(jumpDir * playerStat.JumpForce.Value, ForceMode.Impulse);
        }
    }

    void RotationMouse()
    {
        float mouseX = Mouse.current.delta.x.ReadValue() * sensitivity * Time.deltaTime;
        float mouseY = Mouse.current.delta.y.ReadValue() * sensitivity * Time.deltaTime;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        Quaternion lookRot = Quaternion.Euler(xRotation, yRotation, 0f);
        Quaternion gravityRot = Quaternion.Euler(playerGrafity.currentXRotation, 0f, playerGrafity.currentZRotation);

        transform.rotation = gravityRot * lookRot;
    }
}
