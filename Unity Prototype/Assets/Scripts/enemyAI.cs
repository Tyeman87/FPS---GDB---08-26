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

    [Header("Hearing")]
    [SerializeField] private UnityEngine.UI.Slider stealthBar;
    [SerializeField] private float hearingThreshold = 0.75f;
    [SerializeField] private float hearingDelay = 2f;
    private float hearingTimer = 0f;
    private bool heardPlayer = false;
    private Vector3 noiseLocation;


    [Header("Investigation")]
    [SerializeField] private float investigationRadius = 5f;
    [SerializeField] private float investigationTime = 3f;


    private float investigationTimer = 0f;
    private bool investigating = false;
    private Vector3 searchLocation;
    private Vector3 startingPosition;

    [Header("Stealth Detection")]
    [SerializeField] private float detectionTime = 5f;

    private float detectionTimer = 0f;

    [Header("Detection Indicator")]
    [SerializeField] private GameObject detectionIndicator;

    [SerializeField] private TMPro.TMP_Text detectionText;

    [Header("Detection Risk")]
    [SerializeField] private UnityEngine.UI.Slider detectionRiskBar;

    private void FindDetectionRiskBar()
    {
        if (detectionRiskBar == null)
        {
            DetectionRisk risk = FindAnyObjectByType<DetectionRisk>();

            if (risk != null)
            {
                detectionRiskBar = risk.detectionRiskBar;
            }
        }
    }


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
        if (playerInSight)
        {
            if (canSeePlayer())
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
            Debug.Log("HEARD PLAYER SECTION ACTIVE");
            if (detectionIndicator != null)
            {
                detectionIndicator.SetActive(true);
            }

            if (detectionRiskBar != null)
            {
                detectionRiskBar.value = 0.5f;
            }

            if (detectionText != null)
            {
                detectionText.text = "?";
            }

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
            if (detectionIndicator != null)
            {
                detectionIndicator.SetActive(false);
            }

            checkRoam();
        }

        if (!heardPlayer)
        {
            CheckPlayerNoise();
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
                if (detectionIndicator != null)
                {
                    detectionIndicator.SetActive(true);
                }

                if (detectionText != null)
                {
                    detectionText.text = "!";
                }

                detectionTimer += Time.deltaTime;

                if (detectionRiskBar != null)
                {
                    detectionRiskBar.value = detectionTimer / detectionTime;
                }

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

        detectionTimer = 0f;

        if (detectionRiskBar != null)
        {
            detectionRiskBar.value = 0f;
        }

        if (detectionIndicator != null)
        {
            detectionIndicator.SetActive(false);
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

                if (detectionRiskBar != null)
                {
                    detectionRiskBar.value = 0.5f;
                }

                investigating = false;
                hearingTimer = 0f;

                noiseLocation = gameManager.instance.player.transform.position;

                if (detectionIndicator != null)
                {
                    detectionIndicator.SetActive(true);
                }

                if (detectionText != null)
                {
                    detectionText.text = "?";
                }

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

            agent.SetDestination(startingPos);

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
