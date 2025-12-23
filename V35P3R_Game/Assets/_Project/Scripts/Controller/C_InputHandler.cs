using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class C_InputHandler : MonoBehaviour
{
    private _GrafityPlayer playerGrafity;
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
    UnityEngine.InputSystem.PlayerInput playerInput;
    private InputAction moveAction;
    
    private Vector2 moveInput;
    [SerializeField] private Transform head;
    [SerializeField] private Transform body;

    private void Awake()
    {
        playerGrafity = GetComponent<_GrafityPlayer>();
        //Movement
        playerInput = GetComponent<UnityEngine.InputSystem.PlayerInput>();
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        canMove = true; 
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void OnMove(InputValue value)
    {
        
        moveInput = value.Get<Vector2>();
        
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

    //TODO: add logic returning movement input correspond to player input
    public Vector2 GetMoveInput()
    {
        return moveInput;
    }

    public bool IsJumpPressed()
    {
        return true;
    }

    public bool IsInteractPressed()
    {
        return true;
    }

    public bool IsAbilityPressed()
    {
        return true;
    }
    
    

    void PlayerMove()
    {
        Vector3 gravityDir = playerGrafity.CurrentGravity.normalized;
        Vector3 up = -gravityDir;

        Vector3 forward = Vector3.ProjectOnPlane(head.transform.forward, up).normalized;
        Vector3 right = Vector3.ProjectOnPlane(head.transform.right, up).normalized;

        Vector3 move = forward * moveInput.y + right * moveInput.x;

        float speed = Keyboard.current.leftShiftKey.isPressed ?
            runSpeed :
            walkSpeed;

        rb.MovePosition(rb.position + move * speed * Time.fixedDeltaTime);
        PlayerJump();   
    }
    void PlayerJump()   
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Vector3 jumpDir = -playerGrafity.CurrentGravity.normalized;
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
        Quaternion lookRoty = Quaternion.Euler(0f, yRotation, 0f);
        Quaternion gravityRot = Quaternion.Euler(playerGrafity.currentXRotation, 0f, playerGrafity.currentZRotation);
        transform.rotation = gravityRot;
        head.localRotation =   lookRot;
        body.localRotation = lookRoty;
        
    }
}