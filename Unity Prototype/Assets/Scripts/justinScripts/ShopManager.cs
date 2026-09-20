using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [Header("Shop Items")]
    public ItemStats[] availableItems;

    [Header("Shop UI")]
    public ShopSlot shopSlotTemplate;
    public Transform slotContainer;
    public TextMeshProUGUI playerCreditsText;

    public static ShopManager Instance
    {
        get;
        private set;
    }

    private List<ShopSlot> activeSlots = new List<ShopSlot>();

    private Coroutine flashCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        BuildShop();

        UpdateCreditsUI();

        RefreshShopSlots();
    }

    private void BuildShop()
    {
        foreach (ShopSlot slot in activeSlots)
        {
            if (slot != null)
            {
                Destroy(slot.gameObject);
            }
        }

        activeSlots.Clear();

        if (shopSlotTemplate == null)
        {
            Debug.LogError("ShopManager: Shop Slot Template is not assigned.", this);

            return;
        }

        if (slotContainer == null)
        {
            Debug.LogError("ShopManager: Slot Container is not assigned.", this);

            return;
        }

        if (availableItems == null)
        {
            Debug.LogWarning("ShopManager: No available items assigned.", this);

            return;
        }

        foreach (ItemStats shopItem in availableItems)
        {
            if (shopItem == null)
            {
                continue;
            }

            ShopSlot newSlot = Instantiate(shopSlotTemplate, slotContainer);

            newSlot.shopItem = shopItem;

            newSlot.Initialize();

            activeSlots.Add(newSlot);
        }
    }

    public void UpdateCreditsUI()
    {
        if (playerCreditsText == null)
        {
            return;
        }

        if (SaveDataManager.Instance == null)
        {
            playerCreditsText.text = "Credits: $0";

            return;
        }

        playerCreditsText.text =
            $"Credits: $" +
            $"{SaveDataManager.Instance.PlayerCredits}";
    }

    public void flashCreditsRed()
    {
        if (playerCreditsText == null)
        {
            Debug.LogError(
                "playerCreditsText is not assigned " +
                "in the ShopManager Inspector!",
                this
            );

            return;
        }

        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }

        flashCoroutine = StartCoroutine(FlashRedCoroutine());
    }

    private IEnumerator FlashRedCoroutine()
    {
        playerCreditsText.faceColor = Color.red;

        yield return new WaitForSeconds(0.1f);

        playerCreditsText.faceColor = Color.white;

        flashCoroutine = null;
    }

    public void RefreshShopSlots()
    {
        foreach ( ShopSlot slot in activeSlots)
        {
            if (slot != null)
            {
                slot.RefreshSlotState();
            }
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}