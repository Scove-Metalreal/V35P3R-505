using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_PLayerController : MonoBehaviour
{
    private C_PlayerGrafity playerGrafity;
    private Rigidbody rb;
    public bool canMove = true;
    public float playerSpeed = 5f;
    public float jumpForce = 2f;
    public float runSpeed = 5f;
    public float walkSpeed = 2.5f;
    [Header("Mouselook")]
    public float sensitivity = 200f;
    
    private float xRotation = 0f;private float yRotation = 0f;
    [Header("Movement")]
    private PlayerInput playerInput;
    private InputAction moveAction;
    
    private Vector2 moveInput;

    private void Awake()
    {
        playerGrafity = GetComponent<C_PlayerGrafity>();
        //Movement
        playerInput = new PlayerInput();
        playerInput.Player.Move.performed += ctx =>
        {
            moveInput = ctx.ReadValue<Vector2>();
            
        };
        playerInput.Player.Move.canceled += ctx =>
        {
            moveInput = Vector2.zero;
            
        };
    }
    private void OnEnable() {
        playerInput.Enable();
    }
    private void OnDisable() {
        playerInput.Disable();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        
        
        
        rb = GetComponent<Rigidbody>();
        canMove = true; 
Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        
        RotationMouse();
    }

    private void FixedUpdate()
    {
        if (canMove)
        {
            PlayerMove();
        }

    }

    void PlayerMove()
    {
        
        
        Vector3 gravityDir = Physics.gravity.normalized;
        Vector3 up = -Physics.gravity.normalized;


        Vector3 forward = Vector3.ProjectOnPlane(transform.forward, up).normalized;
        Vector3 right   = Vector3.ProjectOnPlane(transform.right,   up).normalized;
        
        Vector3 move = forward * moveInput.y + right * moveInput.x;

        // rb.linearVelocity = new Vector3(
        //     move.x * playerSpeed,
        //     rb.linearVelocity.y,   
        //     move.z * playerSpeed
        // );
        float speed = Keyboard.current.leftShiftKey.isPressed ? runSpeed : walkSpeed;

        rb.MovePosition(rb.position + move * speed * Time.fixedDeltaTime);
        PlayerJump();   
    }
    void PlayerJump() 
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Vector3 jumpDir = -Physics.gravity.normalized;
            rb.AddForce(jumpDir * jumpForce, ForceMode.Impulse);

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

        
        Quaternion gravityRot = Quaternion.Euler( playerGrafity.currentXRotation, 0f, playerGrafity.currentZRotation);

        
        transform.rotation = gravityRot * lookRot;
    }
}
