using UnityEngine;
using UnityEngine.AI;

public class hostageAI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform player;

    [Header("Follow Settings")]
    [SerializeField] float followDistance = 2f;

    private bool cellOpened = false;

    private void Start()
    {
        // Get NavMeshAgent automatically if one wasn't assigned
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }

        // Find the player
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("Hostage could not find Player!");
        }

        // Make sure the hostage is on the NavMesh
        if (!agent.isOnNavMesh)
        {
            Debug.LogError("HOSTAGE IS NOT ON THE NAVMESH!");
            return;
        }

        // Hostage waits inside the cell
        agent.isStopped = true;

        Debug.Log("Hostage is waiting in cell.");
    }

    private void Update()
    {
        // Don't do anything while the cell is closed
        if (!cellOpened)
        {
            return;
        }

        if (player == null)
        {
            return;
        }

        // Distance between hostage and player
        float distance = Vector3.Distance(
            transform.position,
            player.position
        );

        // Follow player
        if (distance > followDistance)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }
        else
        {
            // Stop when close enough
            agent.isStopped = true;
        }
    }

    public void OpenCell()
    {
        Debug.Log("HOSTAGE CELL OPENED!");

        cellOpened = true;

        if (agent == null)
        {
            Debug.LogError("Hostage has no NavMeshAgent!");
            return;
        }

        if (!agent.isOnNavMesh)
        {
            Debug.LogError("Hostage NavMeshAgent is NOT on NavMesh!");
            return;
        }

        agent.isStopped = false;

        Debug.Log("Hostage is now following the player.");
    }
}