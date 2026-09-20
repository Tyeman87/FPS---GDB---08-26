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

    private StashContainer currentStash;
    private StashContainer.StashedGun selectedGun;
    private CanvasGroup mainUICanvasGroup;

    private void Start()
    {
        stashPanel.SetActive(false);

        if (loadoutPanel != null)
        {
            loadoutPanel.SetActive(false);
        }

        takeButton.onClick.AddListener(
            TakeSelectedGun
        );

        takeButton.interactable = false;
    }

    private void Update()
    {
        // ESC closes the stash/loadout UI completely.
        if (
            stashPanel.activeSelf &&
            Input.GetKeyDown(KeyCode.Escape)
        )
        {
            gameManager.instance
                .SuppressPauseInputThisFrame();

            CloseStash();

            return;
        }

        // ESC while in Loadout returns to the stash.
        if (
            loadoutPanel != null &&
            loadoutPanel.activeSelf &&
            Input.GetKeyDown(KeyCode.Escape)
        )
        {
            gameManager.instance
                .SuppressPauseInputThisFrame();

            CloseLoadout();
        }
    }

    // =========================================================
    // OPEN STASH
    // =========================================================

    public void OpenStash(
        StashContainer stash)
    {
        currentStash = stash;

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

    // =========================================================
    // OPEN LOADOUT
    // =========================================================

    public void OpenLoadout()
    {
        if (loadoutPanel == null)
        {
            Debug.LogWarning(
                "Loadout Panel has not been assigned to StashUI."
            );

            return;
        }

        // Hide stash.
        stashPanel.SetActive(false);

        // Show loadout.
        loadoutPanel.SetActive(true);

        // Keep player disabled.
        player.enabled = false;

        // Keep game paused.
        gameManager.instance.isPaused = true;

        // Keep cursor available for UI.
        UnlockCursor();
    }

    // =========================================================
    // CLOSE LOADOUT / RETURN TO STASH
    // =========================================================

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

        UnlockCursor();
    }

    // =========================================================
    // POPULATE STASH
    // =========================================================

    private void PopulateItemList()
    {
        foreach (Transform child in itemListContent)
        {
            Destroy(child.gameObject);
        }

        List<StashContainer.StashedGun> storedGuns =
            currentStash.GetStoredGuns();

        foreach (
            StashContainer.StashedGun gun
            in storedGuns)
        {
            GameObject newButton =
                Instantiate(
                    stashItemButtonPrefab,
                    itemListContent
                );

            TextMeshProUGUI buttonText =
                newButton.GetComponentInChildren<
                    TextMeshProUGUI
                >();

            if (buttonText != null)
            {
                buttonText.text =
                    gun.stats.name;

                buttonText.raycastTarget =
                    false;
            }

            Button button =
                newButton.GetComponent<Button>();

            if (button != null)
            {
                Debug.Log(
                    "BUTTON CREATED: " +
                    gun.stats.name
                );

                button.onClick.AddListener(() =>
                {
                    Debug.Log(
                        "BUTTON CLICKED: " +
                        gun.stats.name
                    );

                    SelectItem(gun);
                });
            }
        }
    }

    // =========================================================
    // SELECT STASH ITEM
    // =========================================================

    private void SelectItem(
        StashContainer.StashedGun gun)
    {
        Debug.Log(
            "STASH ITEM CLICKED: " +
            gun.stats.name
        );

        selectedGun = gun;

        takeButton.interactable = true;

        selectedItemName.text =
            gun.stats.name;

        selectedItemStats.text =
            $"Magazine: {gun.currMag} / {gun.stats.magSize}\n" +
            $"Reserve: {gun.currReserve} / {gun.stats.maxReserve}\n" +
            $"Damage: {gun.stats.shootDamage}\n" +
            $"Range: {gun.stats.shootDist}";

        ShowPreviewModel(
            gun.stats
        );
    }

    // =========================================================
    // TAKE GUN
    // =========================================================

    private void TakeSelectedGun()
    {
        if (selectedGun == null)
        {
            return;
        }

        player.AddStoredGun(
            selectedGun.stats,
            selectedGun.currMag,
            selectedGun.currReserve
        );

        currentStash.RemoveGun(
            selectedGun
        );

        ClearSelection();

        PopulateItemList();
    }

    // =========================================================
    // CLOSE STASH
    // =========================================================

    public void CloseStash()
    {
        SaveDataManager.Instance
            .SavePlayerInventory(
                player.GetGunInventory(),
                player.GetGunIndex()
            );

        SaveDataManager.Instance
            .SaveStash(
                currentStash.GetStoredGuns()
            );

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

    // =========================================================
    // CURSOR
    // =========================================================

    public void LockCursor()
    {
        Cursor.lockState =
            CursorLockMode.Locked;

        Cursor.visible = false;
    }

    public void UnlockCursor()
    {
        Cursor.lockState =
            CursorLockMode.None;

        Cursor.visible = true;
    }

    // =========================================================
    // MAIN UI
    // =========================================================

    private void HideMainUI()
    {
        if (mainUI == null)
        {
            return;
        }

        mainUICanvasGroup =
            mainUI.GetComponent<CanvasGroup>();

        if (mainUICanvasGroup == null)
        {
            mainUICanvasGroup =
                mainUI.AddComponent<CanvasGroup>();
        }

        mainUICanvasGroup.alpha = 0f;

        mainUICanvasGroup.interactable =
            false;

        mainUICanvasGroup.blocksRaycasts =
            false;
    }

    private void ShowMainUI()
    {
        if (mainUICanvasGroup == null)
        {
            return;
        }

        mainUICanvasGroup.alpha = 1f;

        mainUICanvasGroup.interactable =
            true;

        mainUICanvasGroup.blocksRaycasts =
            true;
    }

    // =========================================================
    // CLEAR SELECTION
    // =========================================================

    private void ClearSelection()
    {
        selectedGun = null;

        takeButton.interactable = false;

        selectedItemName.text = "";

        selectedItemStats.text = "";

        ClearPreviewModel();
    }

    // =========================================================
    // PREVIEW MODEL
    // =========================================================

    private void ShowPreviewModel(
        GunStats gun)
    {
        Transform previewParent =
            GetPreviewModelParent();

        if (
            previewParent == null ||
            gun == null ||
            gun.gunModel == null
        )
        {
            return;
        }

        ClearPreviewModel();

        GameObject previewModel =
            Instantiate(
                gun.gunModel,
                previewParent
            );

        previewModel.transform.localPosition =
            Vector3.zero;

        previewModel.transform.localRotation =
            Quaternion.identity;

        previewModel.transform.localScale =
            Vector3.one;

        SetLayerRecursively(
            previewModel,
            previewParent.gameObject.layer
        );

        DisablePreviewGameplayComponents(
            previewModel
        );
    }

    private void ClearPreviewModel()
    {
        Transform previewParent =
            GetPreviewModelParent();

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

        Transform stashCanvasTransform =
            transform.Find(
                "StashPreviewModel"
            );

        if (stashCanvasTransform != null)
        {
            previewModelParent =
                stashCanvasTransform;
        }

        return previewModelParent;
    }

    private void SetLayerRecursively(
        GameObject obj,
        int layer)
    {
        obj.layer = layer;

        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(
                child.gameObject,
                layer
            );
        }
    }

    private void DisablePreviewGameplayComponents(
        GameObject previewModel)
    {
        foreach (
            Collider collider
            in previewModel.GetComponentsInChildren<Collider>()
        )
        {
            collider.enabled = false;
        }

        foreach (
            Rigidbody rb
            in previewModel.GetComponentsInChildren<Rigidbody>()
        )
        {
            rb.isKinematic = true;
        }

        gunPickup pickup =
            previewModel.GetComponentInChildren<
                gunPickup
            >();

        if (pickup != null)
        {
            pickup.enabled = false;
        }
    }
}