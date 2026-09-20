using System.Collections;
using System.Collections.Generic;
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

    private List<ShopSlot> activeSlots = new List<ShopSlot>();
    private Coroutine flashCoroutine;
    private Color originalTextColor = Color.white;

    [Header("Audio")]
    [SerializeField] AudioClip cashRegSfx;
    [SerializeField] AudioSource cashSrc;
    [Range(0f, 1f)][SerializeField] float cashVol = 0.6f;
    [Range(0.1f, 2f)][SerializeField] float fadeDuration = 0.4f;

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

            activeSlots.Add(newSlot);
        }

        UpdateCreditsUI();

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseShop();
        }
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
        yield return new WaitForSecondsRealtime(0.1f);
        playerCreditsText.faceColor = Color.white;

    }

    public void RefreshShopSlots()
    {
        foreach (ShopSlot slot in activeSlots)
        {
            if (slot != null)
            {
                slot.RefreshSlotState();
            }
        }
    }

    public void playCashRegistSfx()
    {
        if (cashRegSfx == null || cashSrc == null) return;
        cashSrc.clip = cashRegSfx;
        cashSrc.volume = cashVol;
        cashSrc.Play();
        StartCoroutine(FadeOut(cashSrc, fadeDuration));


    }

    IEnumerator FadeOut(AudioSource src, float dur)
    {
        float start = src.volume;
        float elapsed = 0f;
        while (elapsed < dur && src.isPlaying)
        {
            elapsed += Time.unscaledDeltaTime;
            src.volume = Mathf.Lerp(start, 0f, elapsed / dur);
            yield return null;
        }
        src.volume = 0f;
    }

    public void CloseShop()
    {
        Scene shope = SceneManager.GetSceneByName("shopScene");
        SceneManager.UnloadSceneAsync(shope).completed += _ =>
        {
            Scene hub = SceneManager.GetActiveScene();
            SceneManager.SetActiveScene(hub);
            gameManager.instance.stateUnpause();
        };
    }
}
