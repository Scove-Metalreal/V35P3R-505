using Unity.Netcode;
using UnityEngine;

public class M_PlayerStats : NetworkBehaviour
{
    
// HP
    public NetworkVariable<int> Health = new NetworkVariable<int>(
        100,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    // Move speed
    public NetworkVariable<float> WalkSpeed = new NetworkVariable<float>(
        3.5f,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    public NetworkVariable<float> RunSpeed = new NetworkVariable<float>(
        6f,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    // Jump force
    public NetworkVariable<float> JumpForce = new NetworkVariable<float>(
        6f,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    // Gravity rotation sync
    public NetworkVariable<float> GravityXRotation = new NetworkVariable<float>(
        0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public NetworkVariable<float> GravityZRotation = new NetworkVariable<float>(
        0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        Health.OnValueChanged += (oldValue, newValue) =>
        {
            Debug.Log($"HP changed: {newValue}");
        };
    }

    [ServerRpc]
    public void TakeDamageServerRpc(int dmg)
    {
        if (!IsServer) return; 

        Health.Value -= dmg;
        if (Health.Value < 0) Health.Value = 0;
    }
}