using System.Collections;
using UnityEngine.AI;
using UnityEngine;

public class enemyAI : MonoBehaviour, IDamage
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Renderer model;

    [Header("Enemy Stats")]
    [Range(1, 10)][SerializeField] int HP;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int FOV;

    [Header("Weapons")]
    [SerializeField] GameObject bullet;
    [SerializeField] Transform gunPivot;
    [SerializeField] Transform shootPosition;
    [SerializeField] float shootRate;
    [SerializeField] int gunRotateSpeed;

    Color colorOrig;
    Vector3 playerDir;

    float shootTimer;
    bool playerInSight;
    float angleToPlayer;

    void Start()
    {
        colorOrig = model.material.color;
        gameManager.instance.updateGameGoal(1);
    }

    // Update is called once per frame
    void Update()
    {
        if(playerInSight && canSeePlayer())
        {

        }
    }

    bool canSeePlayer()
    {
        // If the player is in the trigger, face the player and shoot at them.
        shootTimer += Time.deltaTime;
        playerDir = gameManager.instance.player.transform.position - transform.position;

        // Calculate the angle between the enemy's forward direction and the direction to the player
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        // Perform a raycast to check if there are any obstacles between the enemy and the player
        RaycastHit hit;
        if (Physics.Raycast(transform.position, playerDir, out hit))
        {
            // If the player is within the enemy's field of view and there are no obstacles, return true
            if (angleToPlayer < FOV && hit.collider.CompareTag("Player"))
            {
                // Set enemy action priorities
                agent.SetDestination(gameManager.instance.player.transform.position);
                faceTarget();
                gunRotation();

                // If the player is in the trigger, shoot at them.
                if (shootTimer >= shootRate)
                {
                    shoot();
                }

                return true;
            }
        }

        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInSight = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInSight = false;
        }
    }

    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, 0, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, faceTargetSpeed * Time.deltaTime);
    }

    void gunRotation()
    {
        Quaternion rot = Quaternion.LookRotation(playerDir);
        gunPivot.rotation = Quaternion.Lerp(gunPivot.rotation, rot, gunRotateSpeed * Time.deltaTime);
    }

    void shoot()
    {
        shootTimer = 0;
        Instantiate(bullet, shootPosition.position, transform.rotation);
    }

    public void takeDamage(int amount)
    {
        HP -= amount;
        agent.SetDestination(gameManager.instance.player.transform.position);

        if (HP <= 0)
        {
            gameManager.instance.updateGameGoal(-1);
            Destroy(gameObject);
        }
        else
        {
            StartCoroutine(flashRed());
        }
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
    }
}
