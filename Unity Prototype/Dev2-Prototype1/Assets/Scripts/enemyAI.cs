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

    void Start()
    {
        HP = maxHP;
        colorOrig = model.material.color;
        agent.speed = moveSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInSight && canSeePlayer())
        {

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
