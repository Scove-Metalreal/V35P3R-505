using UnityEngine;

public class TwoDimentionalBlend : MonoBehaviour
{
    Animator anim;
    float velocityZ = 0.0f;
    float velocityX = 0.0f;
    public float acceleration = 0.5f;
    public float deceleration = 01f;
    public float maximumWalkVelocity = 0.5f;
    public float maximumRunVelocity = 2.0f;

    public C_PlayerInput input;
    Vector2 currentMovement;
    bool movementPressed;
    bool runPressed;
    void Awake()
    {
        input = new C_PlayerInput();

        // set the player input values using listeners
        input.Player.Move.performed += ctx =>
        {
            currentMovement = ctx.ReadValue<Vector2>();
            movementPressed = currentMovement.x != 0 || currentMovement.y != 0;
        };
        input.Player.Move.canceled += ctx =>
        {
            currentMovement = Vector2.zero;
            movementPressed = false;
        };
        input.Player.Sprint.performed += ctx => runPressed = true;
        input.Player.Sprint.canceled += ctx => runPressed = false;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float currentMaxVelocity = runPressed ? maximumRunVelocity : maximumWalkVelocity;

        if (currentMovement.y > 0 && velocityZ < currentMaxVelocity)
        {
            velocityZ += Time.deltaTime * acceleration;
        }
        if (currentMovement.y > 0 && velocityZ < currentMaxVelocity)
        {
            velocityZ += Time.deltaTime * acceleration;
        }
        else
        {
            velocityZ -= Time.deltaTime * deceleration;
        }

        velocityZ = Mathf.Clamp(velocityZ, 0f, currentMaxVelocity);

        // ===== X (Left / Right) =====
        if (Mathf.Abs(currentMovement.x) > 0.01f)
        {
            velocityX += currentMovement.x * Time.deltaTime * acceleration;
        }
        else
        {
            velocityX = Mathf.MoveTowards(
                velocityX,
                0f,
                Time.deltaTime * deceleration
            );
        }

        velocityX = Mathf.Clamp(
            velocityX,
            -currentMaxVelocity,
            currentMaxVelocity
        );
        
        anim.SetFloat("Velocity Z", velocityZ);
        anim.SetFloat("Velocity X", velocityX);
    }
    void OnEnable()
    {
        input.Enable();
    }

    void OnDisable()
    {
        input.Disable();
    }
}
