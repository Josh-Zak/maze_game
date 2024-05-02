using UnityEngine;
using UnityEngine.AI;

public class AIMovement : MonoBehaviour
{

    public NavMeshAgent agent;
    public Transform player;

    private void Update()
    {
        player = GameObject.Find("Player").transform;
        agent.SetDestination(player.position);
        // CheckPathStatus(); // for debug
    }

    void CheckPathStatus()
    {
        switch (agent.pathStatus)
        {
            case NavMeshPathStatus.PathComplete:
                Debug.Log("Path found to destination!");
                break;
            case NavMeshPathStatus.PathPartial:
                Debug.Log("Partial path to destination. Destination not fully reachable.");
                break;
            case NavMeshPathStatus.PathInvalid:
                Debug.Log("No path to destination.");
                break;
        }
    }
}
