using UnityEngine;
using UnityEngine.AI;

public class hostageAI : MonoBehaviour, IDamage, IInteractable
{
    public enum RescueType
    {
        JailCell,
        PlayerProximity,
        PlayerInteraction
    }

    [Header("References")]
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform player;
    [SerializeField] GameObject interactUI;

    [Header("Hostage Stats")]
    [SerializeField] int HP = 8;

    [Header("Rescue Settings")]
    [SerializeField] RescueType rescueType = RescueType.JailCell;

    [SerializeField] float rescueDistance = 5f;

    [Header("Follow Settings")]
    [SerializeField] float followDistance = 2f;

    private bool rescued = false;
    private bool followingPlayer = false;
    public bool IsRescued()
    {
        return rescued;
    }

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
        }

        if (!agent.isOnNavMesh)
        {
            return;
        }

        if (gameManager.instance != null)
        {
            gameManager.instance.RegisterHostage();
        }
        else
        {
        }

        if (rescueType == RescueType.JailCell || rescueType == RescueType.PlayerInteraction)
        {
            agent.isStopped = true;

            if (rescueType == RescueType.PlayerInteraction)
            {
            }
            else
            {
            }
        }

        else if (rescueType == RescueType.PlayerProximity)
        {
            agent.isStopped = true;
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

        if (agent != null)
        {
            Animator anim = GetComponentInChildren<Animator>();
            if (anim != null)
            {
                anim.SetFloat("Speed", agent.velocity.magnitude);
            }
        }
    }

    public void OpenCell()
    {
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
            return;
        }

        if (!agent.isOnNavMesh)
        {
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
        }
    }
    public void takeDamage(int amount)
    {
        HP -= amount;
        if (HP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        missionManager.instance.LoseMission("HOSTAGE KILLED");

        Destroy(gameObject);
    }

    public void Interact()
    {
        if (rescueType != RescueType.PlayerInteraction)
        {
            return;
        }

        if (!rescued)
        {
            RescueHostage();
            return;
        }

        followingPlayer = !followingPlayer;

        if (!followingPlayer)
        {
            agent.isStopped = true;
        }
        else
        {
            agent.isStopped = false;
        }
    }

    public void SetInteractionUI(bool show)
    {
        if (interactUI == null)
        {
            return;
        }

        interactUI.SetActive(show);
    }
}