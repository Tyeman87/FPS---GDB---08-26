using UnityEngine;
using UnityEngine.AI;

public class hostageAI : MonoBehaviour
{
    public enum RescueType
    {
        JailCell,
        PlayerProximity
    }

    [Header("References")]
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform player;

    [Header("Rescue Settings")]
    [SerializeField] RescueType rescueType = RescueType.JailCell;

    [SerializeField] float rescueDistance = 5f;

    [Header("Follow Settings")]
    [SerializeField] float followDistance = 2f;

    private bool rescued = false;
    private bool followingPlayer = false;

    private void Start()
    {
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }

        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("Hostage could not find Player!");
        }

        if (!agent.isOnNavMesh)
        {
            Debug.LogError(
                "HOSTAGE IS NOT ON THE NAVMESH!"
            );

            return;
        }

        if (gameManager.instance != null)
        {
            gameManager.instance.RegisterHostage();
        }
        else
        {
            Debug.LogError("Hostage could not find GameManager!");
        }

        if (rescueType == RescueType.JailCell)
        {
            agent.isStopped = true;

            Debug.Log(
                "Hostage is waiting in jail cell."
            );
        }

        else if (rescueType == RescueType.PlayerProximity)
        {
            agent.isStopped = true;

            Debug.Log(
                "Hostage is waiting for player."
            );
        }
    }

    private void Update()
    {
        if (player == null)
        {
            return;
        }

        if (!rescued &&
            rescueType == RescueType.PlayerProximity)
        {
            float distance = Vector3.Distance(
                transform.position,
                player.position
            );

            if (distance <= rescueDistance)
            {
                RescueHostage();
            }
        }

        if (!followingPlayer)
        {
            return;
        }

        float followDistanceFromPlayer =
            Vector3.Distance(
                transform.position,
                player.position
            );

        if (followDistanceFromPlayer > followDistance)
        {
            agent.isStopped = false;

            agent.SetDestination(
                player.position
            );
        }
        else
        {
            agent.isStopped = true;
        }
    }

    public void OpenCell()
    {
        Debug.Log("HOSTAGE CELL OPENED!");

        if (rescueType != RescueType.JailCell)
        {
            return;
        }

        RescueHostage();
    }

    private void RescueHostage()
    {
        if (rescued)
        {
            return;
        }

        rescued = true;
        followingPlayer = true;

        if (agent == null)
        {
            Debug.LogError(
                "Hostage has no NavMeshAgent!"
            );

            return;
        }

        if (!agent.isOnNavMesh)
        {
            Debug.LogError(
                "Hostage is NOT on the NavMesh!"
            );

            return;
        }

        agent.isStopped = false;

        // Tell GameManager
        if (gameManager.instance != null)
        {
            gameManager.instance.hostageRescued();
        }
        else
        {
            Debug.LogError(
                "Hostage could not find GameManager!"
            );
        }

        Debug.Log(
            "HOSTAGE RESCUED!"
        );
    }
}