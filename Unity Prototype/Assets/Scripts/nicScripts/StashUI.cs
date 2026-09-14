using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class StashUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject stashPanel;
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
        takeButton.onClick.AddListener(TakeSelectedGun);
        takeButton.interactable = false;
    }

    private void Update()
    {
        if (stashPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            gameManager.instance.SuppressPauseInputThisFrame();
            CloseStash();
        }
    }

    public void OpenStash(StashContainer stash)
    {
        currentStash = stash;

        player.enabled = false;
        HideMainUI();
        stashPanel.SetActive(true);

        gameManager.instance.isPaused = true;

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

        List<StashContainer.StashedGun> storedGuns = currentStash.GetStoredGuns();

        foreach (StashContainer.StashedGun gun in storedGuns)
        {
            GameObject newButton = Instantiate(stashItemButtonPrefab, itemListContent);
            TextMeshProUGUI buttonText = newButton.GetComponentInChildren<TextMeshProUGUI>();

            if (buttonText != null)
            {
                buttonText.text = gun.stats.name;
                buttonText.raycastTarget = false;
            }

            Button button = newButton.GetComponent<Button>();

            if (button != null)
            {
                Debug.Log("BUTTON CREATED: " + gun.stats.name);

                button.onClick.AddListener(() =>
                {
                    Debug.Log("BUTTON CLICKED: " + gun.stats.name);
                    SelectItem(gun);
                });
            }
        }
    }

    private void SelectItem(StashContainer.StashedGun gun)
    {
        Debug.Log("STASH ITEM CLICKED: " + gun.stats.name);

        selectedGun = gun;
        takeButton.interactable = true;

        selectedItemName.text = gun.stats.name;

        selectedItemStats.text =
            $"Magazine: {gun.currMag} / {gun.stats.magSize}\n" +
            $"Reserve: {gun.currReserve} / {gun.stats.maxReserve}\n" +
            $"Damage: {gun.stats.shootDamage}\n" +
            $"Range: {gun.stats.shootDist}";

        ShowPreviewModel(gun.stats);
    }

    private void TakeSelectedGun()
    {
        if (selectedGun == null)
        {
            return;
        }

        player.AddStoredGun(selectedGun.stats, selectedGun.currMag, selectedGun.currReserve);
        currentStash.RemoveGun(selectedGun);

        ClearSelection();
        PopulateItemList();
    }

    public void CloseStash()
    {
        player.enabled = true;
        ShowMainUI();
        stashPanel.SetActive(false);

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
        takeButton.interactable = false;

        selectedItemName.text = "";
        selectedItemStats.text = "";
        ClearPreviewModel();
    }

    private void ShowPreviewModel(GunStats gun)
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
    }
}