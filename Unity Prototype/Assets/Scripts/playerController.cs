using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour, IDamage, IPickupGun, IOpen, IMenuBttn
{
    [System.Serializable]
    public class GunAmmoData
    {
        public GunStats stats;
        public int currMag;
        public int currReserve;

        public GunAmmoData(GunStats gun)
        {
            stats = gun;
            currMag = GetUpgradedMagSize(gun);
            currReserve = gun.maxReserve;
        }

        int GetUpgradedMagSize(GunStats gun)
        {
            if (gun == null)
                return 0;

            if (SaveDataManager.Instance == null)
                return gun.magSize;

            WeaponUpgradeData upgradeData =
                SaveDataManager.Instance.GetWeaponUpgradeData(gun.itemID);

            int magSize =
                gun.magSize +
                (gun.magSizeUpgradeAmount * upgradeData.magSizeUpgradeLevel);

            int maxMagSize =
                gun.maxMagSize > 0 ? gun.maxMagSize : gun.magSize;

            return Mathf.Min(magSize, maxMagSize);
        }
    }

    [System.Serializable]
    public class GrenadeInventoryData
    {
        public GrenadeItemStats stats;
        public int count;

        public GrenadeInventoryData(GrenadeItemStats grenade, int amount, int maxAmount)
        {
            stats = grenade;
            count = Mathf.Clamp(amount, 0, maxAmount);
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
    [SerializeField] List<GunStats> startingGuns = new List<GunStats>();
    [SerializeField] GameObject gunModel;

    List<GunAmmoData> gunInv = new List<GunAmmoData>();

    [Header("Grenades")]
    [Tooltip("Optional point where grenades spawn. If empty, the Main Camera is used.")]
    [SerializeField] Transform grenadeThrowPoint;

    [Tooltip("Throw the currently selected grenade.")]
    [SerializeField] KeyCode grenadeThrowKey = KeyCode.R;

    [Tooltip("Switch between grenade types.")]
    [SerializeField] KeyCode grenadeSwitchKey = KeyCode.G;

    [Tooltip("Maximum number of each grenade type the player can carry.")]
    [SerializeField] int maxGrenadesPerType = 3;

    [Tooltip("Reload weapon.")]
    [SerializeField] KeyCode reloadKey = KeyCode.T;

    List<GrenadeInventoryData> grenadeInv = new List<GrenadeInventoryData>();
    int grenadeInvPos;

    [Header("Audio")]
    [SerializeField] AudioClip[] audHurt;
    [Range(0, 1)][SerializeField] float audHurtVol;

    [SerializeField] AudioClip[] audJump;
    [Range(0, 1)][SerializeField] float audJumpVol;

    [SerializeField] AudioClip[] audSteps;
    [Range(0, 1)][SerializeField] float audStepsVol;

    [SerializeField] AudioClip[] reloadSound;
    [Range(0, 1)][SerializeField] float reloadSoundVol = 1f;

    [Header("Interaction")]
    [SerializeField] float interactDistance = 3f;

    int jumpCount;
    int HPOrig;
    int gunInvPos;

    public int keyCount;

    float shootTimer;

    Vector3 moveDir;
    Vector3 playerVel;

    bool isSprinting;
    bool isPlayingStep;

    void Start()
    {
        keyCount = 0;
        HPOrig = HP;

        if (!SaveDataManager.Instance.StartingGearGiven())
        {
            foreach (GunStats gun in startingGuns)
            {
                gunInv.Add(new GunAmmoData(gun));
            }

            gunInvPos = 0;

            SaveDataManager.Instance.MarkStartingGearGiven();
            SaveDataManager.Instance.SavePlayerInventory(gunInv, gunInvPos);

            Debug.Log("Starting gear given to player.");
        }
        else
        {
            SaveDataManager.Instance.LoadPlayerInventory(this);
        }

        spawnPlayer();
        changeGunModel();
    }

    void Update()
    {
        movement();
        sprint();
        interact();
        switchGrenade();
        throwGrenade();
        reload();
        ShowReloadPrompt();
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

        moveDir =
            Input.GetAxis("Horizontal") * transform.right +
            Input.GetAxis("Vertical") * transform.forward;

        characterController.Move(moveDir.normalized * speed * Time.deltaTime);

        jump();

        characterController.Move(playerVel * Time.deltaTime);

        playerVel.y -= gravity * Time.deltaTime;

        if (
            Input.GetButton("Fire1") &&
            gunInv.Count > 0 &&
            shootTimer > GetCurrentShootRate()
        )
        {
            if (gunInv[gunInvPos].currMag > 0)
            {
                shoot();
            }
        }

        selectGun();
    }

    public int GetCurrentDamage()
    {
        if (gunInv.Count == 0)
            return 0;

        GunStats gun = gunInv[gunInvPos].stats;

        WeaponUpgradeData upgradeData =
            SaveDataManager.Instance.GetWeaponUpgradeData(gun.itemID);

        int damage =
            gun.shootDamage +
            (gun.damageUpgradeAmount * upgradeData.damageUpgradeLevel);

        int maxDamage =
            gun.maxDamage > 0 ? gun.maxDamage : damage;

        return Mathf.Min(damage, maxDamage);
    }

    public float GetCurrentShootRate()
    {
        if (gunInv.Count == 0)
            return 1f;

        GunStats gun = gunInv[gunInvPos].stats;

        WeaponUpgradeData upgradeData =
            SaveDataManager.Instance.GetWeaponUpgradeData(gun.itemID);

        float shootRate =
            gun.shootRate -
            (gun.shootRateUpgradeAmount * upgradeData.shootRateUpgradeLevel);

        float minShootRate =
            gun.minShootRate > 0 ? gun.minShootRate : 0.1f;

        return Mathf.Max(shootRate, minShootRate);
    }

    public int GetCurrentMagSize()
    {
        if (gunInv.Count == 0)
            return 0;

        GunStats gun = gunInv[gunInvPos].stats;

        WeaponUpgradeData upgradeData =
            SaveDataManager.Instance.GetWeaponUpgradeData(gun.itemID);

        int magSize =
            gun.magSize +
            (gun.magSizeUpgradeAmount * upgradeData.magSizeUpgradeLevel);

        int maxMagSize =
            gun.maxMagSize > 0 ? gun.maxMagSize : gun.magSize;

        return Mathf.Min(magSize, maxMagSize);
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

        if (
            audioManager.Instance != null &&
            audioManager.Instance.audPlayer != null &&
            audSteps != null &&
            audSteps.Length > 0
        )
        {
            audioManager.Instance.audPlayer.PlayOneShot(
                audSteps[Random.Range(0, audSteps.Length)],
                audStepsVol
            );
        }

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

            if (
                audioManager.Instance != null &&
                audioManager.Instance.audPlayer != null &&
                audJump != null &&
                audJump.Length > 0
            )
            {
                audioManager.Instance.audPlayer.PlayOneShot(
                    audJump[Random.Range(0, audJump.Length)],
                    audJumpVol
                );
            }
        }
    }

    void shoot()
    {
        shootTimer = 0;
        gunInv[gunInvPos].currMag--;

        updatePlayerUI();

        GunStats gun = gunInv[gunInvPos].stats;

        if (
            audioManager.Instance != null &&
            audioManager.Instance.audPlayer != null &&
            gun.shootSound != null &&
            gun.shootSound.Length > 0
        )
        {
            audioManager.Instance.audPlayer.PlayOneShot(
                gun.shootSound[Random.Range(0, gun.shootSound.Length)],
                gun.shootSoundVol
            );
        }

        RaycastHit hit;

        if (
            Physics.Raycast(
                Camera.main.transform.position,
                Camera.main.transform.forward,
                out hit,
                gun.shootDist,
                ~ignoreLayer,
                QueryTriggerInteraction.Ignore
            )
        )
        {
            if (gun.hitEffect != null)
            {
                Instantiate(gun.hitEffect, hit.point, Quaternion.identity);
            }

            IDamage dmg = hit.collider.GetComponentInParent<IDamage>();

            if (dmg != null)
            {
                dmg.takeDamage(GetCurrentDamage());
            }
        }
    }

    void switchGrenade()
    {
        if (!Input.GetKeyDown(grenadeSwitchKey))
            return;

        if (grenadeInv.Count == 0)
        {
            Debug.Log("GRENADE: No grenades available.");
            return;
        }

        if (grenadeInv.Count == 1)
        {
            GrenadeInventoryData onlyGrenade = grenadeInv[0];

            Debug.Log(
                "GRENADE SELECTED: " +
                onlyGrenade.stats.itemName +
                " x" +
                onlyGrenade.count
            );

            return;
        }

        grenadeInvPos++;

        if (grenadeInvPos >= grenadeInv.Count)
        {
            grenadeInvPos = 0;
        }

        GrenadeInventoryData selectedGrenade = grenadeInv[grenadeInvPos];

        Debug.Log(
            "GRENADE SWITCHED TO: " +
            selectedGrenade.stats.itemName +
            " x" +
            selectedGrenade.count
        );
    }

    void throwGrenade()
    {
        if (!Input.GetKeyDown(grenadeThrowKey))
            return;

        if (grenadeInv.Count == 0)
        {
            Debug.Log("GRENADE: No grenades available.");
            return;
        }

        grenadeInvPos = Mathf.Clamp(grenadeInvPos, 0, grenadeInv.Count - 1);

        GrenadeInventoryData grenadeData = grenadeInv[grenadeInvPos];

        if (grenadeData == null || grenadeData.stats == null)
        {
            Debug.LogWarning("GRENADE: Current grenade is null.");
            return;
        }

        GrenadeItemStats grenade = grenadeData.stats;

        if (grenadeData.count <= 0)
        {
            grenadeInv.RemoveAt(grenadeInvPos);
            FixGrenadeIndex();
            return;
        }

        if (grenade.grenadePrefab == null)
        {
            Debug.LogError(
                grenade.itemName +
                " does not have a Grenade Prefab assigned."
            );

            return;
        }

        Camera cam = Camera.main;

        if (cam == null)
        {
            Debug.LogError("GRENADE: Main Camera was not found.");
            return;
        }

        Vector3 throwPosition;

        if (grenadeThrowPoint != null)
        {
            throwPosition = grenadeThrowPoint.position;
        }
        else
        {
            throwPosition =
                cam.transform.position +
                cam.transform.forward * 0.75f;
        }

        Vector3 targetPoint =
            cam.transform.position +
            cam.transform.forward * grenade.throwRange;

        if (
            Physics.Raycast(
                cam.transform.position,
                cam.transform.forward,
                out RaycastHit hit,
                grenade.throwRange,
                ~ignoreLayer,
                QueryTriggerInteraction.Ignore
            )
        )
        {
            targetPoint = hit.point;
        }

        GameObject grenadeObject =
            Instantiate(
                grenade.grenadePrefab,
                throwPosition,
                Quaternion.identity
            );

        GrenadeProjectile projectile =
            grenadeObject.GetComponent<GrenadeProjectile>();

        if (projectile == null)
        {
            Debug.LogError(
                grenade.itemName +
                " prefab needs a GrenadeProjectile component."
            );

            Destroy(grenadeObject);
            return;
        }

        Rigidbody rb = grenadeObject.GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError(
                grenade.itemName +
                " prefab needs a Rigidbody."
            );

            Destroy(grenadeObject);
            return;
        }

        projectile.Initialize(grenade, gameObject);

        rb.linearVelocity =
            CalculateGrenadeVelocity(
                throwPosition,
                targetPoint
            );

        rb.angularVelocity = Random.insideUnitSphere * 8f;

        grenadeData.count--;

        Debug.Log(
            "THREW GRENADE: " +
            grenade.itemName +
            " | Remaining: " +
            grenadeData.count +
            " | Damage: " +
            grenade.damage +
            " | Radius: " +
            grenade.blastRadius +
            " | Range: " +
            grenade.throwRange
        );

        if (grenadeData.count <= 0)
        {
            string emptyGrenadeName = grenade.itemName;

            grenadeInv.RemoveAt(grenadeInvPos);
            FixGrenadeIndex();

            if (grenadeInv.Count > 0)
            {
                GrenadeInventoryData nextGrenade = grenadeInv[grenadeInvPos];

                Debug.Log(
                    emptyGrenadeName +
                    " depleted. Switched to " +
                    nextGrenade.stats.itemName +
                    " x" +
                    nextGrenade.count
                );
            }
            else
            {
                Debug.Log(emptyGrenadeName + " depleted. No grenades remaining.");
            }
        }
    }

    void FixGrenadeIndex()
    {
        if (grenadeInv.Count == 0)
        {
            grenadeInvPos = 0;
            return;
        }

        if (grenadeInvPos >= grenadeInv.Count)
        {
            grenadeInvPos = 0;
        }

        grenadeInvPos = Mathf.Clamp(grenadeInvPos, 0, grenadeInv.Count - 1);
    }

    Vector3 CalculateGrenadeVelocity(Vector3 start, Vector3 target)
    {
        Vector3 displacement = target - start;

        Vector3 horizontal =
            new Vector3(
                displacement.x,
                0f,
                displacement.z
            );

        float horizontalDistance = horizontal.magnitude;

        float flightTime =
            Mathf.Clamp(
                horizontalDistance / 12f,
                0.45f,
                1.25f
            );

        Vector3 horizontalVelocity = horizontal / flightTime;

        float verticalVelocity =
            (
                displacement.y -
                0.5f *
                Physics.gravity.y *
                flightTime *
                flightTime
            ) /
            flightTime;

        return horizontalVelocity + Vector3.up * verticalVelocity;
    }

    void reload()
    {
        if (Input.GetKeyDown(reloadKey) && gunInv.Count > 0)
        {
            GunAmmoData gun = gunInv[gunInvPos];
            int currentMagSize = GetCurrentMagSize();

            if (gun.currMag >= currentMagSize || gun.currReserve <= 0)
            { 
                return; 
            }


            int roundsNeeded = currentMagSize - gun.currMag;
            int reloadAmt = Mathf.Min(roundsNeeded, gun.currReserve);

            gun.currMag += reloadAmt;
            gun.currReserve -= reloadAmt;

            if (
                audioManager.Instance != null &&
                audioManager.Instance.audPlayer != null &&
                reloadSound != null &&
                reloadSound.Length > 0
            )
            {
                audioManager.Instance.audPlayer.PlayOneShot(
                    reloadSound[Random.Range(0, reloadSound.Length)],
                    reloadSoundVol
                );
            }

            updatePlayerUI();

        }
        else
        {
            return;
        }

    }

    public void takeDamage(int amount)
    {
        int absorbed = Mathf.Min(armor, amount);

        armor -= absorbed;
        HP -= amount - absorbed;

        updatePlayerUI();

        StartCoroutine(flashDamage());

        if (
            audioManager.Instance != null &&
            audioManager.Instance.audPlayer != null &&
            audHurt != null &&
            audHurt.Length > 0
        )
        {
            audioManager.Instance.audPlayer.PlayOneShot(
                audHurt[Random.Range(0, audHurt.Length)],
                audHurtVol
            );
        }

        if (HP <= 0)
        {
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
        gameManager.instance.playerHPBar.fillAmount =
            (float)HP / HPOrig;

        gameManager.instance.playerArmorBar.fillAmount =
            (float)armor / armorMax;

        if (gunInv.Count > 0)
        {
            gameManager.instance.ammoCounterText.text =
                $"{gunInv[gunInvPos].currMag} / " +
                $"{gunInv[gunInvPos].currReserve}";
        }
        else
        {
            gameManager.instance.ammoCounterText.text = "NO WEAPON";
        }
    }

    public void addHealth(int amount)
    {
        HP += amount;

        if (HP > HPOrig)
        {
            HP = HPOrig;
        }

        updatePlayerUI();
    }

    public void addArmor(int amount)
    {
        armor += amount;

        if (armor > armorMax)
        {
            armor = armorMax;
        }

        updatePlayerUI();
    }

    public void spawnPlayer()
    {
        characterController.transform.position =
            gameManager.instance.playerSpawnPos.transform.position;

        Physics.SyncTransforms();

        HP = HPOrig;

        updatePlayerUI();
    }

    void interact()
    {
        if (!Input.GetButtonDown("Interact"))
            return;

        Ray ray =
            new Ray(
                Camera.main.transform.position,
                Camera.main.transform.forward
            );

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, ~ignoreLayer, QueryTriggerInteraction.Ignore))
        {
            IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>();

            if (interactable != null)
            {
                interactable.Interact();
            }
        }
    }

    public void getGunStats(GunStats gun)
    {
        for (int i = 0; i < gunInv.Count; i++)
        {
            if (gunInv[i].stats == gun)
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

    public void AddStoredGun(GunStats gun, int currMag, int currReserve)
    {
        if (gun == null)
            return;

        GunAmmoData storedGun = new GunAmmoData(gun);

        int maxMagSize = GetCurrentMagSizeForGun(gun);

        storedGun.currMag =
            Mathf.Clamp(
                currMag,
                0,
                maxMagSize
            );

        storedGun.currReserve =
            Mathf.Clamp(
                currReserve,
                0,
                gun.maxReserve
            );

        gunInv.Add(storedGun);

        gunInvPos = gunInv.Count - 1;

        changeGunModel();
        updatePlayerUI();
    }

    int GetCurrentMagSizeForGun(GunStats gun)
    {
        if (gun == null)
            return 0;

        if (SaveDataManager.Instance == null)
            return gun.magSize;

        WeaponUpgradeData upgradeData =
            SaveDataManager.Instance.GetWeaponUpgradeData(gun.itemID);

        int magSize =
            gun.magSize +
            (gun.magSizeUpgradeAmount * upgradeData.magSizeUpgradeLevel);

        int maxMagSize =
            gun.maxMagSize > 0 ? gun.maxMagSize : gun.magSize;

        return Mathf.Min(magSize, maxMagSize);
    }

    public GunAmmoData GetCurrentGun()
    {
        if (gunInv.Count == 0)
            return null;

        return gunInv[gunInvPos];
    }

    public void RemoveCurrentGun()
    {
        if (gunInv.Count == 0)
            return;

        gunInv.RemoveAt(gunInvPos);

        if (gunInv.Count > 0)
        {
            if (gunInvPos >= gunInv.Count)
            {
                gunInvPos = gunInv.Count - 1;
            }

            changeGunModel();
        }
        else
        {
            if (gunModel != null)
            {
                gunModel.SetActive(false);
            }

            gunInvPos = 0;
        }

        updatePlayerUI();
    }

    void changeGunModel()
    {
        if (gunModel == null)
            return;

        if (gunInv.Count == 0)
        {
            gunModel.SetActive(false);
            return;
        }

        GunStats currentGun = gunInv[gunInvPos].stats;

        if (currentGun == null || currentGun.gunModel == null)
        {
            gunModel.SetActive(false);
            return;
        }

        MeshFilter sourceMesh =
            currentGun.gunModel.GetComponent<MeshFilter>();

        MeshRenderer sourceRenderer =
            currentGun.gunModel.GetComponent<MeshRenderer>();

        MeshFilter targetMesh =
            gunModel.GetComponent<MeshFilter>();

        MeshRenderer targetRenderer =
            gunModel.GetComponent<MeshRenderer>();

        if (
            sourceMesh == null ||
            sourceRenderer == null ||
            targetMesh == null ||
            targetRenderer == null
        )
        {
            Debug.LogWarning(
                "Gun model is missing a MeshFilter or MeshRenderer."
            );

            return;
        }

        gunModel.SetActive(true);

        targetMesh.sharedMesh = sourceMesh.sharedMesh;
        targetRenderer.sharedMaterial = sourceRenderer.sharedMaterial;
    }

    void selectGun()
    {
        if (gunInv.Count == 0)
            return;

        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll > 0 && gunInvPos < gunInv.Count - 1)
        {
            gunInvPos++;

            changeGunModel();
            updatePlayerUI();
        }
        else if (scroll < 0 && gunInvPos > 0)
        {
            gunInvPos--;

            changeGunModel();
            updatePlayerUI();
        }
    }

    public void ClearGunInventory()
    {
        gunInv.Clear();
        gunInvPos = 0;

        if (gunModel != null)
        {
            gunModel.SetActive(false);
        }

        updatePlayerUI();
    }

    public void SetGunIndex(int index)
    {
        if (gunInv.Count == 0)
        {
            gunInvPos = 0;

            changeGunModel();
            updatePlayerUI();

            return;
        }

        gunInvPos =
            Mathf.Clamp(
                index,
                0,
                gunInv.Count - 1
            );

        changeGunModel();
        updatePlayerUI();
    }

    public void FillAmmo(int ammoAmount)
    {
        if (gunInv.Count == 0)
            return;

        GunAmmoData gun = gunInv[gunInvPos];

        gun.currReserve =
            Mathf.Min(
                gun.currReserve + ammoAmount,
                gun.stats.maxReserve
            );
        gun.currMag = gun.stats.magSize;
        updatePlayerUI();
    }

    public void AddGrenade(GrenadeItemStats grenade, int amount = 1)
    {
        if (grenade == null || amount <= 0)
            return;

        foreach (GrenadeInventoryData existingGrenade in grenadeInv)
        {
            if (existingGrenade.stats == grenade)
            {
                existingGrenade.count =
                    Mathf.Min(
                        existingGrenade.count + amount,
                        maxGrenadesPerType
                    );

                Debug.Log(
                    grenade.itemName +
                    " x" +
                    existingGrenade.count
                );

                return;
            }
        }

        GrenadeInventoryData newGrenade =
            new GrenadeInventoryData(
                grenade,
                amount,
                maxGrenadesPerType
            );

        grenadeInv.Add(newGrenade);

        Debug.Log(
            "Added grenade: " +
            grenade.itemName +
            " x" +
            newGrenade.count
        );
    }

    public void ClearGrenadeInventory()
    {
        grenadeInv.Clear();
        grenadeInvPos = 0;
    }

    public List<GrenadeInventoryData> GetGrenadeInventory()
    {
        return grenadeInv;
    }

    public GrenadeItemStats GetCurrentGrenade()
    {
        if (grenadeInv.Count == 0)
            return null;

        grenadeInvPos =
            Mathf.Clamp(
                grenadeInvPos,
                0,
                grenadeInv.Count - 1
            );

        return grenadeInv[grenadeInvPos].stats;
    }

    public int GetCurrentGrenadeAmount()
    {
        if (grenadeInv.Count == 0)
            return 0;

        grenadeInvPos =
            Mathf.Clamp(
                grenadeInvPos,
                0,
                grenadeInv.Count - 1
            );

        return grenadeInv[grenadeInvPos].count;
    }

    public int GetGrenadeTypeCount()
    {
        return grenadeInv.Count;
    }

    public int GetGrenadeCount()
    {
        int total = 0;

        foreach (GrenadeInventoryData grenade in grenadeInv)
        {
            if (grenade != null)
            {
                total += grenade.count;
            }
        }

        return total;
    }

    public int GetGrenadeIndex()
    {
        return grenadeInvPos;
    }

    void ShowReloadPrompt()
    {
        if (gameManager.instance == null)
            return;

        if (gameManager.instance.reloadPopup == null)
            return;

        if (gunInv.Count == 0)
        {
            gameManager.instance.reloadPopup.SetActive(false);
            return;
        }

        if (
            gunInv[gunInvPos].currMag == 0 &&
            gunInv[gunInvPos].currReserve > 0
        )
        {
            gameManager.instance.reloadPopup.SetActive(true);
        }
        else
        {
            gameManager.instance.reloadPopup.SetActive(false);
        }
    }

    public List<GunAmmoData> GetGunInventory()
    {
        return gunInv;
    }

    public int GetGunIndex()
    {
        return gunInvPos;
    }

    public int GetAmmoAmountToAdd()
    {
        return (gunInv[gunInvPos].stats.magSize - gunInv[gunInvPos].currMag) +
            (gunInv[gunInvPos].stats.maxReserve - gunInv[gunInvPos].currReserve);
    }
}