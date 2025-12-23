using Unity.VisualScripting;
using UnityEngine;

public class C_CombatControl : MonoBehaviour
{
    public C_BlendTreeController blend;
    public int punches = 0;

    public bool hurricaneKick = false;
    public bool elbow = false;
    public bool ribHit = false;

    public void Start()
    {
        blend.anim = GetComponent<Animator>();
    }
    void Punching()
    {
        blend.anim.SetBool("CombatState", true);
        if (punches > 3)
        {
            PunchCombo();
        }
        if (hurricaneKick)
        {
            HurricaneKick();
        }
        if (elbow)
        {
            Elbow();
        }
        if (ribHit)
        {
            RibHit();
        }
    }
    void PunchCombo()
    {
        blend.anim.SetTrigger("PunchCombo");
    }
    void HurricaneKick()
    {
        blend.anim.SetTrigger("HurricaneKick");
    }
    void Elbow()
    {
        blend.anim.SetTrigger("Elbow");
    }
    void RibHit()
    {
        blend.anim.SetTrigger("RibHit");
    }
}
