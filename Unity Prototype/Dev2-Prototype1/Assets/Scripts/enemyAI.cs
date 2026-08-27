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

    [Header("Weapons")]
    [SerializeField] GameObject bullet;
    [SerializeField] Transform gunPivot;
    [SerializeField] Transform shootPosition;
    [SerializeField] float shootRate;
    [SerializeField] int gunRotateSpeed;
    [SerializeField] int bulletDamage;

    public Color colorOrig;
    Vector3 playerDir;
    

    float shootTimer;
    bool playerInSight;
    float angleToPlayer;

    
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
    }

    // Update is called once per frame
    void Update()
    {
        

        if (playerInSight && canSeePlayer())
        {

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
        playerDir = gameManager.instance.player.transform.position - transform.position;

        angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, playerDir.normalized, out hit))
        {
            Debug.DrawRay(transform.position, playerDir.normalized * hit.distance, Color.green);
            if (angleToPlayer < FOV && hit.collider.CompareTag("Player"))
            {
                agent.SetDestination(gameManager.instance.player.transform.position);
                faceTarget();
                gunRotation();

                if (shootTimer >= shootRate)
                {
                    shoot();
                }

                return true;
            }
        }
        else
        {
            Debug.DrawRay(transform.position, playerDir.normalized, Color.red);
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

        GameObject newBullet = Instantiate(bullet, shootPosition.position, gunPivot.rotation);

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
