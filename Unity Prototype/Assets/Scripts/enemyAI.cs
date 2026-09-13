using System.Collections;
using UnityEngine.AI;
using UnityEngine;

public class enemyAI : MonoBehaviour, IDamage
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] public Renderer model;

    [Header("Enemy Stats")]
    [Range(1, 10)][SerializeField] public int HP;
    [SerializeField] public int maxHP;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int FOV;
    [SerializeField] float moveSpeed;
    [SerializeField] int roamDist;
    [SerializeField] int roamPauseTime;
    [SerializeField] Transform eyePosition;

    [Header("Weapons")]
    [SerializeField] GameObject bullet;
    [SerializeField] Transform gunPivot;
    [SerializeField] Transform shootPosition;
    [SerializeField] float shootRate;
    [SerializeField] int gunRotateSpeed;
    [SerializeField] int bulletDamage;

    [Header("Attack Types")]
    [SerializeField] bool isRanged;

    public Color colorOrig;
    Vector3 playerDir;

    assaultMode assaultMode;
    

    float shootTimer;
    bool playerInSight;
    float angleToPlayer;
    int playerTriggerCount;


    float roamTimer;
    float stoppingDistOrig;
    bool playerInTrigger;
    Vector3 startingPos;

    Vector3 lastPos;
    float stuckTimer;
    //float stuckThreshold = 0.5f;

    void Start()
    {
        HP = maxHP;
        colorOrig = model.material.color;
        agent.speed = moveSpeed;
        startingPos = transform.position;
        assaultMode = FindAnyObjectByType<assaultMode>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInSight)
        {
            if (canSeePlayer())
            {
                if (isRanged)
                {
                    agent.stoppingDistance = 10f;
                    agent.SetDestination(gameManager.instance.player.transform.position);
                }
            }
        }
        else
        {
            checkRoam();
        }
    }

    void checkRoam()
    {
        if(agent.remainingDistance < 0.1f)
        {
            roamTimer += Time.deltaTime;
            if (roamTimer > roamPauseTime)
            {
                roam();
            }
        }
    }

    void roam()
    {
        roamTimer = 0;
        agent.stoppingDistance = 0;

        Vector3 randPos = Random.insideUnitSphere * roamDist;
        randPos += startingPos;
        NavMeshHit hit;

        if (NavMesh.SamplePosition(randPos, out hit, roamDist, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }


    bool canSeePlayer()
    {
        if (!agent.enabled || !agent.isOnNavMesh) return false;
        shootTimer += Time.deltaTime;
        playerDir = gameManager.instance.player.transform.position - eyePosition.position;

        Vector3 flatPlayerDir = new Vector3(playerDir.x, 0, playerDir.z);
        angleToPlayer = Vector3.Angle(flatPlayerDir, transform.forward);

        RaycastHit hit;

        if (Physics.Raycast(
            eyePosition.position,
            playerDir.normalized,
            out hit,
            playerDir.magnitude
        ))
        {
            if (angleToPlayer < FOV &&
                hit.collider.GetComponentInParent<playerController>() != null)
            {
                if (isRanged)
                {
                    agent.stoppingDistance = 10f;
                    faceTarget();
                    gunRotation();

                    if (shootTimer >= shootRate)
                    {
                        shoot();
                    }
                }
                else
                {
                    agent.stoppingDistance = 0;
                    agent.SetDestination(gameManager.instance.player.transform.position);
                    faceTarget();
                }

                return true;
            }
        }

        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<playerController>() != null)
        {
            playerTriggerCount++;
            playerInSight = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponentInParent<playerController>() != null)
        {
            playerTriggerCount--;

            if (playerTriggerCount <= 0)
            {
                playerTriggerCount = 0;
                playerInSight = false;
            }
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

        Vector3 targetDirection =
            gameManager.instance.player.transform.position
            - shootPosition.position;

        Quaternion targetRotation =
            Quaternion.LookRotation(targetDirection);

        GameObject newBullet = Instantiate(
            bullet,
            shootPosition.position,
            targetRotation
        );

        damage bulletDamageScript = newBullet.GetComponent<damage>();

        if (bulletDamageScript != null)
        {
            bulletDamageScript.setDamage(bulletDamage);
        }
    }

    public void takeDamage(int amount)
    {
        HP -= amount;
        if (agent.enabled && agent.isOnNavMesh)
        { 
            agent.SetDestination(gameManager.instance.player.transform.position); 
        }

        if (HP <= 0)
        {
            gameManager.instance.addKill();

            if (gameStats.Instance != null)
            {
                gameStats.Instance.EnemyKilled();
            }

            if (assaultMode != null)
            {
                assaultMode.enemyDefeated();
            }

            RespawnManager.instance.HandleEnemyDeath(gameObject);
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
