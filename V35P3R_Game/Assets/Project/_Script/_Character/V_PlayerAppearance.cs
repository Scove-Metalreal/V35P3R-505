using UnityEngine;

public class V_PlayerAppearance : MonoBehaviour
{

    Animator anim;

    private bool isWalking = false;
    private bool isRunning = false;

    void Start()
    {
        anim = GetComponent<Animator>();
    }
    private void Update()
    {
        // if (isWalking)
        // anim.SetBool("isWalking", true);
        // if (isRunning)
        // anim.SetBool("isRunning", false);
    }
}
