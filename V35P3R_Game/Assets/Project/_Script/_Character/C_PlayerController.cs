using UnityEngine;
using UnityEngine.InputSystem;

public class C_PlayerController : MonoBehaviour
{
    public M_PlayerData playerData;
    PlayerInput input;

    Vector2 moveInput;

    private void Awake()
    {
        input = new PlayerInput();
    }
    public void OnMove()
    {
        
    }
    public void OnEnable()
    {
        
    }
    public void OnDisable()
    {
        
    }
}
