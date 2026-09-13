using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopSlot : MonoBehaviour
{
    public ItemStats shopItem;
    //ref to name, text, and buy button
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI priceText;
    public Button buyButton;

    public void Initialize()
    {
        itemName.text = shopItem.itemName;
        priceText.text = $"${shopItem.itemCost}";

        if (buyButton != null)
        {
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(OnPurchaseClicked);
        }
        else
        {
            Debug.LogError($"buyButton is NOT assigned on {gameObject.name}!", this);
        }

        RefreshSlotState();
    }
    private void Start()
    {
        RefreshSlotState();
    }

    public void OnPurchaseClicked()
    {
        if (SaveDataManager.Instance.IsItemUnlocked(shopItem.itemID)) return;

        bool success = SaveDataManager.Instance.Purchase(shopItem);

        if (success)
        {
            ShopManager.Instance.UpdateCreditsUI();
            RefreshSlotState();

        }
        else
        {
            ShopManager.Instance.flashCreditsRed();
        }

    }

    private void RefreshSlotState()
    {
        if (shopItem != null && SaveDataManager.Instance != null)
        {
            bool isUnlocked = SaveDataManager.Instance.IsItemUnlocked(shopItem.itemID);
            if (isUnlocked)
            {
                buyButton.interactable = false;
                TextMeshProUGUI btnText = buyButton.GetComponentInChildren<TextMeshProUGUI>();
                if (btnText != null)
                {
                    btnText.text = "Owned";
                }
            }
        }
    }
}


