using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour, IDamage, IPickupGun
{
    [SerializeField] CharacterController characterController;
    [SerializeField] LayerMask ignoreLayer;

    [Header("Player Stats")]
    [Range(1, 10)] [SerializeField]int HP;
    [Range(1, 10)] [SerializeField]int speed;
    [Range(2, 10)] [SerializeField]int sprintMod;
    [Range(5, 30)] [SerializeField]int jumpSpeed;
    [Range(1, 5)] [SerializeField]int jumpMax;
    [Range(15, 40)] [SerializeField]int gravity;

    [Header("GunStff")]
    [SerializeField] List<gunStats> gunInv = new List<gunStats>();
    [SerializeField] GameObject gunModel;

    [Header("Audio")]
    [SerializeField] AudioClip[] audHurt;
    [Range(0, 1)][SerializeField] float audHurtVol;
    [SerializeField] AudioClip[] audJump;
    [Range(0, 1)][SerializeField] float audJumpVol;
    [SerializeField] AudioClip[] audSteps;
    [Range(0, 1)][SerializeField] float audStepsVol;

    [Header("Interaction")]
    [SerializeField] float interactDistance = 3f;

    int jumpCount;
    int HPOrig;
    int gunInvPos;

    float shootTimer;

    Vector3 moveDir;
    Vector3 playerVel;

    bool isSprinting;
    bool isPlayingStep;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HPOrig = HP;
        spawnPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        movement();
        sprint();
        interact();
    }

    void movement()
    {
        if (gunInv.Count > 0)
        {
            Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * gunInv[gunInvPos].shootDist, Color.red);
        }
        shootTimer += Time.deltaTime;
        
        if (characterController.isGrounded)
        {
            jumpCount = 0;
            playerVel.y = 0;

            if(moveDir.magnitude > 0.3f && !isPlayingStep)
            {
                StartCoroutine(playStep());
            }
        }

        moveDir = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;
        characterController.Move(moveDir.normalized * speed * Time.deltaTime);

        jump();
        characterController.Move(playerVel * Time.deltaTime);
        playerVel.y -= gravity * Time.deltaTime;

        if (Input.GetButton("Fire1") && gunInv.Count > 0 && shootTimer > gunInv[gunInvPos].shootRate)
        {
            shoot();
        }

        selectGun();
    }

    void sprint()
    {
        if(Input.GetButtonDown("Sprint"))
        {
            speed *= sprintMod;
            isSprinting = true;
        }
        else if(Input.GetButtonUp("Sprint"))
        {
            speed /= sprintMod;
            isSprinting = false;
        }
    }

    IEnumerator playStep()
    {
        isPlayingStep = true;
        audioManager.Instance.audPlayer.PlayOneShot(audSteps[Random.Range(0, audSteps.Length)], audStepsVol);

        if(isSprinting)
        {
            yield return new WaitForSeconds(0.3f);
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
        }

        isPlayingStep = false;
    }

    void jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            jumpCount++;
            playerVel.y = jumpSpeed;
            audioManager.Instance.audPlayer.PlayOneShot(audJump[Random.Range(0, audJump.Length)], audJumpVol);
        }
    }

    void shoot()
    {
        shootTimer = 0;
        audioManager.Instance.audPlayer.PlayOneShot(gunInv[gunInvPos].shootSound[Random.Range(0, gunInv[gunInvPos].shootSound.Length)], gunInv[gunInvPos].shootSoundVol);

        RaycastHit hit;
        if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, gunInv[gunInvPos].shootDist, ~ignoreLayer))
        {
            Debug.Log(hit.collider.name);

            Instantiate(gunInv[gunInvPos].hitEffect, hit.point, Quaternion.identity);
            IDamage dmg = hit.collider.GetComponent<IDamage>();
            if(dmg != null)
            {
                dmg.takeDamage(gunInv[gunInvPos].shootDamage);
            }
        }
    }

    public void takeDamage(int amount)
    {
        HP -= amount; 
        updatePlayerUI();
        StartCoroutine(flashDamage());

        audioManager.Instance.audPlayer.PlayOneShot(audHurt[Random.Range(0, audHurt.Length)], audHurtVol);

        if(HP <= 0)
        {
            // I'm dead!!!
            gameManager.instance.youLose();
        }
    }

    IEnumerator flashDamage()
    {
        gameManager.instance.damageFlashPanel.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gameManager.instance.damageFlashPanel.SetActive(false);
    }

    public void updatePlayerUI()
    {
        gameManager.instance.playerHPBar.fillAmount = (float)HP / HPOrig;
    }

    public void spawnPlayer()
    {
        characterController.transform.position = gameManager.instance.playerSpawnPos.transform.position;
        Physics.SyncTransforms();
        HP = HPOrig;
        updatePlayerUI();
    }
    
    void interact()
    {
        if (Input.GetButtonDown("Interact"))
        {
            Ray ray = new Ray(
                Camera.main.transform.position,
                Camera.main.transform.forward
            );

            if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, ~ignoreLayer))
            {
                IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>();

                if (interactable != null)
                {
                    interactable.Interact();
                }
            }
        }
    }

    public void getGunStats(gunStats gun)
    {
        gunInv.Add(gun);
        gunInvPos = gunInv.Count - 1;
        changeGunModel();
    }

    void changeGunModel()
    {
        gunModel.GetComponent<MeshFilter>().sharedMesh = gunInv[gunInvPos].gunModel.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunInv[gunInvPos].gunModel.GetComponent<MeshRenderer>().sharedMaterial;
    }

    void selectGun()
    {
        if(Input.GetAxis("Mouse ScrollWheel") > 0 && gunInvPos < gunInv.Count - 1)
        {
            gunInvPos++;
            changeGunModel();
        }

        else if(Input.GetAxis("Mouse ScrollWheel") < 0 && gunInvPos > 0)
        {
            gunInvPos--;
            changeGunModel();
        }
    }
}
