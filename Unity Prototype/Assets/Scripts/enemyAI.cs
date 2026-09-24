using System.Collections;
using UnityEngine.AI;
using UnityEngine;

public class enemyAI : MonoBehaviour, IDamage
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] public Renderer model;
    [SerializeField] Animator animator;

    [Header("Enemy Stats")]
    [Range(1, 100)][SerializeField] public int HP;
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

    [Header("Hearing")]
    [SerializeField] private UnityEngine.UI.Slider stealthBar;
    [SerializeField] private float hearingThreshold = 0.75f;
    [SerializeField] private float hearingDelay = 2f;
    private float hearingTimer = 0f;
    private bool heardPlayer = false;
    private Vector3 noiseLocation;
    bool isDead;

    [SerializeField] float deathDespawnDelay = 3f;


    [Header("Investigation")]
    [SerializeField] private float investigationRadius = 5f;
    [SerializeField] private float investigationTime = 3f;


    private float investigationTimer = 0f;
    private bool investigating = false;
    private Vector3 searchLocation;

    [Header("Stealth Detection")]
    [SerializeField] private float detectionTime = 5f;


    [Header("Audio")]
    [SerializeField] AudioClip[] audHurt;
    [Range(0, 1)][SerializeField] float audHurtVol;
    [SerializeField] AudioClip[] audDeath;
    [Range(0, 1)][SerializeField] float audDeathVol;
    [SerializeField] AudioClip[] gunshotClips;


    private float detectionTimer = 0f;


    public Color colorOrig;
    Vector3 playerDir;

    assaultMode assaultMode;
    protectMode protectMode;
    


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
        protectMode = FindAnyObjectByType<protectMode>();
    }

    // Update is called once per frame
    void Update()
    {
        bool seesPlayer = playerInSight && canSeePlayer();

        if (seesPlayer)
        {
            if (seesPlayer)
            {
                heardPlayer = false;
                investigating = false;
                investigationTimer = 0f;

                if (!isRanged)
                {
                    agent.stoppingDistance = 0f;
                    agent.SetDestination(gameManager.instance.player.transform.position);
                }
                else
                {
                    agent.stoppingDistance = 10f;
                    agent.SetDestination(gameManager.instance.player.transform.position);
                }
            }
        }
        else if (heardPlayer)
        {
            Debug.Log("Heard player. On NavMesh: " + agent.isOnNavMesh);

            if (agent.isOnNavMesh)
            {
                if (!investigating)
                {
                    agent.SetDestination(noiseLocation);

                    if (!agent.pathPending && agent.remainingDistance <= 2f)
                    {
                        investigating = true;
                        investigationTimer = 0f;
                    }
                }
                else
                {
                    SearchNoiseArea();
                }
            }
        }
        else
        {
            checkRoam();
        }

        if (!heardPlayer)
        {
            CheckPlayerNoise();
        }

        if (animator != null)
        {
            bool moving = agent != null
                && agent.enabled
                && agent.isOnNavMesh
                && agent.velocity.sqrMagnitude > 0.01f;

            animator.SetBool("IsWalking", moving);
            animator.SetBool("IsAiming", isRanged && seesPlayer && !moving);
        }

        if (isDead)
        {
            return;
        }
    }

    void checkRoam()
    {
        if (agent.isOnNavMesh && agent.remainingDistance < 0.1f)
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
        Debug.Log("Player distance: " + playerDir.magnitude);
        angleToPlayer = Vector3.Angle(flatPlayerDir, transform.forward);

        RaycastHit hit;

        if (Physics.Raycast(
            eyePosition.position,
            playerDir.normalized,
            out hit,
            playerDir.magnitude
        ))
        {
            Debug.Log("Raycast hit: " + hit.collider.name);
            if (angleToPlayer < FOV &&
                hit.collider.GetComponentInParent<playerController>() != null)
            {
                detectionTimer += Time.deltaTime;

                if (detectionTimer >= detectionTime)
                {
                    if (missionManager.instance != null)
                    {
                        missionManager.instance.LoseMission("STEALTH DETECTED");
                    }

                    detectionTimer = 0f;
                }

                if (isRanged)
                {
                    agent.stoppingDistance = 10f;
                    faceTarget();

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

        detectionTimer = 0f;

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

        if (audioManager.Instance != null && gunshotClips.Length > 0)
        {
            audioManager.Instance.audPlayer.PlayOneShot(gunshotClips[Random.Range(0, gunshotClips.Length)]);
        }

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

        foreach (Collider bulletCollider in newBullet.GetComponentsInChildren<Collider>())
        {
            foreach (Collider enemyCollider in GetComponentsInChildren<Collider>())
            {
                Physics.IgnoreCollision(bulletCollider, enemyCollider);
            }
        }

        damage bulletDamageScript = newBullet.GetComponent<damage>();

        if (bulletDamageScript != null)
        {
            bulletDamageScript.setDamage(bulletDamage);
        }
    }

    public void takeDamage(int amount)
    {
        if (isDead)
        {
            return;
        }
        HP -= amount;

        if (agent.enabled && agent.isOnNavMesh)
        {
            agent.SetDestination(gameManager.instance.player.transform.position);
        }

        if (HP <= 0)
        {
            if (gameManager.instance != null)
            {
                gameManager.instance.addKill();
            }

            if (gameStats.Instance != null)
            {
                gameStats.Instance.EnemyKilled();
            }

            if (assaultMode != null)
            {
                assaultMode.enemyDefeated();
            }

           
            if (protectMode != null)
            {
                protectMode.enemyDefeated();
            }
			
            isDead = true;
            StopAllCoroutines();

            if (model != null)
            {
                model.material.color = colorOrig;
            }

            if (agent != null && agent.enabled)
            {
                if (agent.isOnNavMesh)
                {
                    agent.isStopped = true;
                    agent.ResetPath();
                }

                agent.enabled = false;
            }

            foreach (Collider col in GetComponentsInChildren<Collider>())
            {
                col.enabled = false;
            }

            if (animator != null)
            {
                animator.SetBool("IsWalking", false);
                animator.SetBool("IsAiming", false);
                animator.SetTrigger("Die");
            }
            Destroy(gameObject, deathDespawnDelay);

            audioManager.Instance.audPlayer.PlayOneShot(audDeath[Random.Range(0, audDeath.Length)], audDeathVol);

        }
        else
        {
            StartCoroutine(flashRed());
        }

        DmgCounter.instance.addDmg(amount);

        audioManager.Instance.audPlayer.PlayOneShot(audHurt[Random.Range(0, audHurt.Length)], audHurtVol);
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
    }

    private void CheckPlayerNoise()
    {
        if (stealthBar == null)
            return;

        if (stealthBar.value >= hearingThreshold)
        {
            hearingTimer += Time.deltaTime;

            if (hearingTimer >= hearingDelay)
            {
                heardPlayer = true;
                investigating = false;
                hearingTimer = 0f;

                noiseLocation = gameManager.instance.player.transform.position;

                Debug.Log("Enemy heard the player!");
            }
        }
        else
        {
            hearingTimer = 0f;
        }
    }

    private void SearchNoiseArea()
    {
        Debug.Log("SEARCHING NOISE AREA");
        investigationTimer += Time.deltaTime;

        if (investigationTimer >= investigationTime)
        {
            investigating = false;
            heardPlayer = false;
            hearingTimer = 0f;

            agent.ResetPath();

            return;
        }

        if (!agent.hasPath || agent.remainingDistance <= 0.5f)
        {
            Vector3 randomPoint = noiseLocation + Random.insideUnitSphere * investigationRadius;
            randomPoint.y = noiseLocation.y;

            UnityEngine.AI.NavMeshHit hit;

            if (UnityEngine.AI.NavMesh.SamplePosition(randomPoint, out hit, investigationRadius, UnityEngine.AI.NavMesh.AllAreas))
            {
                searchLocation = hit.position;
                agent.SetDestination(searchLocation);
            }
        }
    }
}
