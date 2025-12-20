using UnityEngine;
using UnityEngine.AI;

public class C_NPCMoving : MonoBehaviour
{
    private NavMeshAgent agent;
    public Transform playerTarget;
    void Start()
    {
        playerTarget = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(playerTarget.position);
    }
}
