using UnityEngine;
using UnityEngine.AI;

public class C_NPCMoving : MonoBehaviour
{
    private NavMeshAgent agent;
    public Transform playerTarget;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        playerTarget = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(playerTarget.position);
    }
}
