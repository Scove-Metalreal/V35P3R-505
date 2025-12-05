using UnityEngine;

public class Test_PLayerController : MonoBehaviour
{
    private Rigidbody rb;
    public bool canMove = true;
    public float playerSpeed = 5f;
    public float grafityScale = 2f;

    public float sensitivity = 200f;

    private float xRotation = 0f;private float yRotation = 0f;
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
        if (canMove)
        {
            PlayerMove();
        }

        rotationMouse();
    }

    void PlayerMove()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = (transform.right * x + transform.forward * z).normalized;

        rb.linearVelocity = new Vector3(
            move.x * playerSpeed,
            rb.linearVelocity.y,   
            move.z * playerSpeed
        );
        if ( Input.GetKey(KeyCode.LeftShift))
        {
            playerSpeed = 5f;
        }
        else
        {
            playerSpeed = 2.5f;
        }
        PlayerJump();   
    }
    void PlayerJump() 
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * grafityScale,ForceMode.Impulse);
        }
    }

    void rotationMouse()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);
yRotation += mouseX;
         transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);

        
         // transform.Rotate(Vector3.up * mouseX);
    }
}
