using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class StashUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject stashPanel;
    [SerializeField] private GameObject loadoutPanel;
    [SerializeField] private Transform itemListContent;
    [SerializeField] private TextMeshProUGUI selectedItemName;
    [SerializeField] private TextMeshProUGUI selectedItemStats;
    [SerializeField] private Button takeButton;
    [SerializeField] private GameObject mainUI;
    [SerializeField] private playerController player;
    [SerializeField] private GameObject stashItemButtonPrefab;
    [SerializeField] private Transform previewModelParent;

    [Header("Grenades")]
    [SerializeField] private int grenadesGivenWhenTaken = 3;

    private StashContainer currentStash;
    private StashContainer.StashedGun selectedGun;
    private GrenadeItemStats selectedGrenade;
    private CanvasGroup mainUICanvasGroup;

    private void Start()
    {
        stashPanel.SetActive(false);

        if (loadoutPanel != null)
        {
            loadoutPanel.SetActive(false);
        }

        takeButton.onClick.AddListener(TakeSelectedItem);
        takeButton.interactable = false;
    }

    private void Update()
    {
        if (stashPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            gameManager.instance.SuppressPauseInputThisFrame();
            CloseStash();
            return;
        }

        if (loadoutPanel != null && loadoutPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            gameManager.instance.SuppressPauseInputThisFrame();
            CloseLoadout();
        }
    }

    public void OpenStash(StashContainer stash)
    {
        currentStash = stash;

        if (currentStash != null)
        {
            currentStash.RefreshFromLoadout();
        }

        player.enabled = false;

        HideMainUI();

        if (loadoutPanel != null)
        {
            loadoutPanel.SetActive(false);
        }

        stashPanel.SetActive(true);

        gameManager.instance.isPaused = true;

        ClearSelection();
        PopulateItemList();
        UnlockCursor();
    }

    public void OpenLoadout()
    {
        if (loadoutPanel == null)
        {
            Debug.LogWarning("Loadout Panel has not been assigned to StashUI.");
            return;
        }

        stashPanel.SetActive(false);
        loadoutPanel.SetActive(true);

        player.enabled = false;
        gameManager.instance.isPaused = true;

        UnlockCursor();
    }

    public void CloseLoadout()
    {
        if (loadoutPanel == null)
        {
            return;
        }

        loadoutPanel.SetActive(false);
        stashPanel.SetActive(true);

        player.enabled = false;
        gameManager.instance.isPaused = true;

        if (currentStash != null)
        {
            currentStash.RefreshFromLoadout();
        }

        ClearSelection();
        PopulateItemList();

        UnlockCursor();
    }

    private void PopulateItemList()
    {
        foreach (Transform child in itemListContent)
        {
            Destroy(child.gameObject);
        }

        if (currentStash != null)
        {
            currentStash.RefreshFromLoadout();

            List<StashContainer.StashedGun> storedGuns = currentStash.GetStoredGuns();

            foreach (StashContainer.StashedGun gun in storedGuns)
            {
                if (gun == null || gun.stats == null)
                {
                    continue;
                }

                CreateGunButton(gun);
            }
        }

        if (SaveDataManager.Instance != null)
        {
            List<GrenadeItemStats> unlockedGrenades = SaveDataManager.Instance.GetUnlockedGrenades();

            foreach (GrenadeItemStats grenade in unlockedGrenades)
            {
                if (grenade == null)
                {
                    continue;
                }

                CreateGrenadeButton(grenade);
            }
        }
    }

    private void CreateGunButton(StashContainer.StashedGun gun)
    {
        GameObject newButton = Instantiate(stashItemButtonPrefab, itemListContent);

        TextMeshProUGUI buttonText = newButton.GetComponentInChildren<TextMeshProUGUI>();

        if (buttonText != null)
        {
            buttonText.text = gun.stats.itemName;
            buttonText.raycastTarget = false;
        }

        Button button = newButton.GetComponent<Button>();

        if (button != null)
        {
            StashContainer.StashedGun buttonGun = gun;
            button.onClick.AddListener(() => SelectGun(buttonGun));
        }
    }

    private void CreateGrenadeButton(GrenadeItemStats grenade)
    {
        GameObject newButton = Instantiate(stashItemButtonPrefab, itemListContent);

        TextMeshProUGUI buttonText = newButton.GetComponentInChildren<TextMeshProUGUI>();

        if (buttonText != null)
        {
            buttonText.text = grenade.itemName;
            buttonText.raycastTarget = false;
        }

        Button button = newButton.GetComponent<Button>();

        if (button != null)
        {
            GrenadeItemStats buttonGrenade = grenade;
            button.onClick.AddListener(() => SelectGrenade(buttonGrenade));
        }
    }

    private void SelectGun(StashContainer.StashedGun gun)
    {
        if (gun == null || gun.stats == null)
        {
            return;
        }

        selectedGun = gun;
        selectedGrenade = null;

        takeButton.interactable = true;

        selectedItemName.text = gun.stats.itemName;

        selectedItemStats.text =
            "Type: Gun" +
            "\nMagazine: " + gun.currMag +
            "\nReserve: " + gun.currReserve +
            "\nDamage: " + gun.stats.shootDamage +
            "\nRange: " + gun.stats.shootDist;

        ShowGunPreviewModel(gun.stats);
    }

    private void SelectGrenade(GrenadeItemStats grenade)
    {
        if (grenade == null)
        {
            return;
        }

        selectedGun = null;
        selectedGrenade = grenade;

        takeButton.interactable = true;

        selectedItemName.text = grenade.itemName;

        selectedItemStats.text =
            "Type: Grenade" +
            "\nDamage: " + grenade.damage +
            "\nBlast Radius: " + grenade.blastRadius.ToString("0.0") +
            "\nThrow Range: " + grenade.throwRange.ToString("0.0") +
            "\nAmount Given: " + grenadesGivenWhenTaken;

        ShowGrenadePreviewModel(grenade);
    }

    private void TakeSelectedItem()
    {
        if (selectedGun != null)
        {
            TakeSelectedGun();
            return;
        }

        if (selectedGrenade != null)
        {
            TakeSelectedGrenade();
        }
    }

    private void TakeSelectedGun()
    {
        if (selectedGun == null || selectedGun.stats == null)
        {
            return;
        }

        List<playerController.GunAmmoData> playerGuns = player.GetGunInventory();

        for (int i = 0; i < playerGuns.Count; i++)
        {
            if (playerGuns[i].stats == selectedGun.stats)
            {
                playerGuns[i].currMag = selectedGun.currMag;
                playerGuns[i].currReserve = selectedGun.currReserve;

                player.SetGunIndex(i);

                Debug.Log("Lobby test gun refreshed: " + selectedGun.stats.itemName);

                return;
            }
        }

        player.AddStoredGun(selectedGun.stats, selectedGun.currMag, selectedGun.currReserve);

        Debug.Log("Lobby test gun given: " + selectedGun.stats.itemName);
    }

    private void TakeSelectedGrenade()
    {
        if (selectedGrenade == null)
        {
            return;
        }

        player.AddGrenade(selectedGrenade, grenadesGivenWhenTaken);

        if (gameManager.instance != null)
        {
            gameManager.instance.UpdateGrenadeUI();
        }

        Debug.Log("Lobby test grenades given: " + selectedGrenade.itemName + " x" + grenadesGivenWhenTaken);
    }

    public void CloseStash()
    {
        player.enabled = true;

        ShowMainUI();

        stashPanel.SetActive(false);

        if (loadoutPanel != null)
        {
            loadoutPanel.SetActive(false);
        }

        gameManager.instance.isPaused = false;

        ClearSelection();
        LockCursor();
    }

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void HideMainUI()
    {
        if (mainUI == null)
        {
            return;
        }

        mainUICanvasGroup = mainUI.GetComponent<CanvasGroup>();

        if (mainUICanvasGroup == null)
        {
            mainUICanvasGroup = mainUI.AddComponent<CanvasGroup>();
        }

        mainUICanvasGroup.alpha = 0f;
        mainUICanvasGroup.interactable = false;
        mainUICanvasGroup.blocksRaycasts = false;
    }

    private void ShowMainUI()
    {
        if (mainUICanvasGroup == null)
        {
            return;
        }

        mainUICanvasGroup.alpha = 1f;
        mainUICanvasGroup.interactable = true;
        mainUICanvasGroup.blocksRaycasts = true;
    }

    private void ClearSelection()
    {
        selectedGun = null;
        selectedGrenade = null;

        takeButton.interactable = false;

        selectedItemName.text = "";
        selectedItemStats.text = "";

        ClearPreviewModel();
    }

    private void ShowGunPreviewModel(GunStats gun)
    {
        Transform previewParent = GetPreviewModelParent();

        if (previewParent == null || gun == null || gun.gunModel == null)
        {
            return;
        }

        ClearPreviewModel();

        GameObject previewModel = Instantiate(gun.gunModel, previewParent);

        previewModel.transform.localPosition = Vector3.zero;
        previewModel.transform.localRotation = Quaternion.identity;
        previewModel.transform.localScale = Vector3.one;

        SetLayerRecursively(previewModel, previewParent.gameObject.layer);
        DisablePreviewGameplayComponents(previewModel);
    }

    private void ShowGrenadePreviewModel(GrenadeItemStats grenade)
    {
        Transform previewParent = GetPreviewModelParent();

        if (previewParent == null || grenade == null)
        {
            return;
        }

        GameObject grenadeModel = grenade.grenadeModel;

        if (grenadeModel == null)
        {
            grenadeModel = grenade.grenadePrefab;
        }

        if (grenadeModel == null)
        {
            return;
        }

        ClearPreviewModel();

        GameObject previewModel = Instantiate(grenadeModel, previewParent);

        previewModel.transform.localPosition = Vector3.zero;
        previewModel.transform.localRotation = Quaternion.identity;
        previewModel.transform.localScale = Vector3.one;

        SetLayerRecursively(previewModel, previewParent.gameObject.layer);
        DisablePreviewGameplayComponents(previewModel);
    }

    private void ClearPreviewModel()
    {
        Transform previewParent = GetPreviewModelParent();

        if (previewParent == null)
        {
            return;
        }

        foreach (Transform child in previewParent)
        {
            Destroy(child.gameObject);
        }
    }

    private Transform GetPreviewModelParent()
    {
        if (previewModelParent != null)
        {
            return previewModelParent;
        }

        Transform stashCanvasTransform = transform.Find("StashPreviewModel");

        if (stashCanvasTransform != null)
        {
            previewModelParent = stashCanvasTransform;
        }

        return previewModelParent;
    }

    private void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;

        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }

    private void DisablePreviewGameplayComponents(GameObject previewModel)
    {
        foreach (Collider collider in previewModel.GetComponentsInChildren<Collider>())
        {
            collider.enabled = false;
        }

        foreach (Rigidbody rb in previewModel.GetComponentsInChildren<Rigidbody>())
        {
            rb.isKinematic = true;
        }

        gunPickup pickup = previewModel.GetComponentInChildren<gunPickup>();

        if (pickup != null)
        {
            pickup.enabled = false;
        }

        GrenadeProjectile grenadeProjectile = previewModel.GetComponentInChildren<GrenadeProjectile>();

        if (grenadeProjectile != null)
        {
            grenadeProjectile.enabled = false;
        }
    }
}