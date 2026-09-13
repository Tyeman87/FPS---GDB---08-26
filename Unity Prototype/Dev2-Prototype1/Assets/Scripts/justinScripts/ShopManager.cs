using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShopManager : MonoBehaviour
{
    public ItemStats[] availableItems;
    public ShopSlot shopSlotTemplate;
    public Transform slotContainer;
    public static ShopManager Instance { get; private set; }
    public TextMeshProUGUI playerCreditsText;
    private Coroutine flashCoroutine;
    private Color originalTextColor = Color.white;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {

        //loop through availableItems
        foreach (ItemStats shopItem in availableItems)
        {
            //instantiate each slot as a child of the container
            ShopSlot newSlot = Instantiate(shopSlotTemplate, slotContainer);
            //set the shop item
            newSlot.shopItem = shopItem;
            //initialize
            newSlot.Initialize();
        }

        UpdateCreditsUI();

    }

    public void UpdateCreditsUI()
    {
        if (playerCreditsText != null && SaveDataManager.Instance != null)
        {
            playerCreditsText.text = $"Credits: ${SaveDataManager.Instance.PlayerCredits}";
        }
    }

    public void flashCreditsRed()
    {
        if (playerCreditsText == null)
        {
            Debug.LogError("playerCreditsText is not assigned in the ShopManager Inspector!", this);
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
        yield return new WaitForSeconds(0.3f);
        playerCreditsText.faceColor = Color.white;
    }
}
