using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour, IDamage, IPickupGun, IOpen
{
    //for each gun's ammo on the player
    [System.Serializable]
    public class GunAmmoData
    {
        public gunStats stats;
        public int currMag;
        public int currReserve;

        public GunAmmoData(gunStats gun)
        {
            stats = gun;
            currMag = gun.magSize;
            currReserve = gun.maxReserve;
        }
    }
    [SerializeField] CharacterController characterController;
    [SerializeField] LayerMask ignoreLayer;

    [Header("Player Stats")]
    [Range(1, 30)][SerializeField] int HP;
    [Range(0, 20)][SerializeField] int armor;
    [Range(1, 20)][SerializeField] int armorMax;
    [Range(1, 10)][SerializeField] int speed;
    [Range(2, 10)][SerializeField] int sprintMod;
    [Range(5, 30)][SerializeField] int jumpSpeed;
    [Range(1, 5)][SerializeField] int jumpMax;
    [Range(15, 40)][SerializeField] int gravity;

    [Header("GunStuff")]
    [SerializeField] List<gunStats> startingGuns = new List<gunStats>();//inventory at beginning of level
    [SerializeField] GameObject gunModel;
    List<GunAmmoData> gunInv = new List<GunAmmoData>();// to hold stats + ammo of held guns

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
        foreach (gunStats gun in startingGuns)
        {
            gunInv.Add(new GunAmmoData(gun));
        }
        spawnPlayer();

        if (gunInv.Count > 0)
        {
            changeGunModel();
        }

    }

    // Update is called once per frame
    void Update()
    {
        movement();
        sprint();
        interact();
        reload();
    }

    void movement()
    {
        shootTimer += Time.deltaTime;

        if (characterController.isGrounded)
        {
            jumpCount = 0;
            playerVel.y = 0;

            if (moveDir.magnitude > 0.3f && !isPlayingStep)
            {
                StartCoroutine(playStep());
            }
        }

        moveDir = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;
        characterController.Move(moveDir.normalized * speed * Time.deltaTime);

        jump();
        characterController.Move(playerVel * Time.deltaTime);
        playerVel.y -= gravity * Time.deltaTime;

        if (Input.GetButton("Fire1") && gunInv.Count > 0 && shootTimer > gunInv[gunInvPos].stats.shootRate)
        {
            if (gunInv[gunInvPos].currMag > 0) //check if there's ammo in mag to shoot
            {
                shoot();
            }
        }

        selectGun();
    }

    void sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            speed *= sprintMod;
            isSprinting = true;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            speed /= sprintMod;
            isSprinting = false;
        }
    }

    IEnumerator playStep()
    {
        isPlayingStep = true;
        audioManager.Instance.audPlayer.PlayOneShot(audSteps[Random.Range(0, audSteps.Length)], audStepsVol);

        if (isSprinting)
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
        gunInv[gunInvPos].currMag--;//subtract ammo when shooting
        
        updatePlayerUI();

        gunStats gun = gunInv[gunInvPos].stats;

        audioManager.Instance.audPlayer.PlayOneShot(gunInv[gunInvPos].stats.shootSound[Random.Range(0, gunInv[gunInvPos].stats.shootSound.Length)], gunInv[gunInvPos].stats.shootSoundVol);

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, gunInv[gunInvPos].stats.shootDist, ~ignoreLayer, QueryTriggerInteraction.Ignore))
        {
            Instantiate(gunInv[gunInvPos].stats.hitEffect, hit.point, Quaternion.identity);
            IDamage dmg = hit.collider.GetComponentInParent<IDamage>();

            if (dmg != null)
            {
                dmg.takeDamage(gunInv[gunInvPos].stats.shootDamage);
            }
            else
            {
            }
        }
    }

    void reload()
    {
        if (Input.GetKeyDown(KeyCode.R) && gunInv.Count > 0)
        {
            GunAmmoData gun = gunInv[gunInvPos];

            if (gun.currMag >= gun.stats.magSize || gun.currReserve <= 0) return;

            int roundsNeeded = gun.stats.magSize - gun.currMag;
            int reloadAmt = Mathf.Min(roundsNeeded, gun.currReserve);

            gun.currMag += reloadAmt;
            gun.currReserve -= reloadAmt;
            Debug.Log($"Reloaded {reloadAmt} rounds");

            updatePlayerUI();//update player ui so they see the changes from the reload
        }
    }
    

    public void takeDamage(int amount)
    {
        //subtract armor amount first

        int absorbed = Mathf.Min(armor, amount);
        armor -= absorbed;

        HP -= (amount - absorbed);

        updatePlayerUI();
        StartCoroutine(flashDamage());

        audioManager.Instance.audPlayer.PlayOneShot(audHurt[Random.Range(0, audHurt.Length)], audHurtVol);

        if (HP <= 0)
        {
            // I'm dead!!!
            missionManager.instance.LoseMission("PLAYER KILLED");
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
        gameManager.instance.playerArmorBar.fillAmount = (float)armor / armorMax;
        gameManager.instance.ammoCounterText.text = $"{gunInv[gunInvPos].currMag} / {gunInv[gunInvPos].currReserve}";
        
    }

    public void addHealth(int amount)
    {
        HP += amount;
        if (HP > HPOrig)
        {
            HP = HPOrig;
        }
    }

    public void addArmor(int amount)
    {
        armor += amount;
        if (armor > armorMax)
        {
            armor = armorMax;
        }
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
        for(int i = 0; i < gunInv.Count; i++)
        {
            if (gunInv[i].stats == gun)//if gun picked up is already in inventory, fill ammo
            {
                gunInv[i].currReserve = gun.maxReserve;
                gunInvPos = i;
                changeGunModel();
                return;
            }
        }

        gunInv.Add(new GunAmmoData(gun));
        gunInvPos = gunInv.Count - 1;
        changeGunModel();
    }

    void changeGunModel()
    {
        gunModel.GetComponent<MeshFilter>().sharedMesh = gunInv[gunInvPos].stats.gunModel.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunInv[gunInvPos].stats.gunModel.GetComponent<MeshRenderer>().sharedMaterial;
    }

    void selectGun()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && gunInvPos < gunInv.Count - 1)
        {
            gunInvPos++;
            changeGunModel();
        }

        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && gunInvPos > 0)
        {
            gunInvPos--;
            changeGunModel();
        }
        updatePlayerUI();
    }
}
