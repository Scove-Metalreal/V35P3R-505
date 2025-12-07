using UnityEngine;

public class C_PlayerGrafity : MonoBehaviour
{
    public enum GravityDirection
    {
        Up,
        Down,
        Left,
        Right
    }

    public bool GrafityDown = true;
    public bool GrafityUp = false;
    public bool GrafityLeft = false;
    public bool GrafityRight = false;

    public bool isGround = true;
    private bool isChangingGravity = false;
    public float currentZRotation = 0f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
    }

    void Update()
    {
        // Đổi trọng lực bằng Ctrl + WASD
        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (Input.GetKeyDown(KeyCode.W))
                ApplyGravityDirection(GravityDirection.Up);

            if (Input.GetKeyDown(KeyCode.S))
                ApplyGravityDirection(GravityDirection.Down);

            if (Input.GetKeyDown(KeyCode.A))
                ApplyGravityDirection(GravityDirection.Left);

            if (Input.GetKeyDown(KeyCode.D))
                ApplyGravityDirection(GravityDirection.Right);
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
            case GravityDirection.Up:
                Physics.gravity = Vector3.up * 9.81f;
                currentZRotation = 180f;
                GrafityUp = true; GrafityDown = GrafityLeft = GrafityRight = false;
                break;

            case GravityDirection.Left:
                Physics.gravity = Vector3.left * 9.81f;
                currentZRotation = -90f;
                GrafityLeft = true; GrafityDown = GrafityUp = GrafityRight = false;
                break;

            case GravityDirection.Right:
                Physics.gravity = Vector3.right * 9.81f;
                currentZRotation = 90f;
                GrafityRight = true; GrafityDown = GrafityUp = GrafityLeft = false;
                break;

            case GravityDirection.Down:
            default:
                Physics.gravity = Vector3.down * 9.81f;
                currentZRotation = 0f;
                GrafityDown = true; GrafityUp = GrafityLeft = GrafityRight = false;
                break;
        }

        StartGravityChange();
    }
}
