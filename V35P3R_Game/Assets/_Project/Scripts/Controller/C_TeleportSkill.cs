using UnityEngine;
using UnityEngine.InputSystem;

public class C_TeleportSkill : MonoBehaviour
{
    public float maxDistance = 15f;
    public LayerMask groundLayer;
    public Transform cameraTransform;
    public GameObject previewMarker;

    private Rigidbody rb;
    private bool teleportMode;
    private bool canTeleport;
    private Vector3 targetPosition;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (teleportMode)
            Aim();
        if (Keyboard.current.qKey.wasPressedThisFrame) return;

        teleportMode = !teleportMode;

        if (!teleportMode)
            HidePreview();
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (teleportMode && canTeleport)
            {
                rb.position = targetPosition;
                teleportMode = false;
                HidePreview();
            }
        }
    }

    public void OnTeleportSkill(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        teleportMode = !teleportMode;

        if (!teleportMode)
            HidePreview();
    }

    public void OnTeleportConfirm(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (teleportMode && canTeleport)
        {
            rb.position = targetPosition;
            teleportMode = false;
            HidePreview();
        }
    }

    void Aim()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistance, groundLayer))
        {
            if (hit.normal.y < 0.6f)
            {
                canTeleport = false;
                previewMarker.SetActive(false);
                return;
            }

            canTeleport = true;
            targetPosition = hit.point;

            previewMarker.SetActive(true);
            previewMarker.transform.position = hit.point;
        }
        else
        {
            canTeleport = false;
            HidePreview();
        }
    }

    void HidePreview()
    {
        canTeleport = false;
        previewMarker.SetActive(false);
    }
}