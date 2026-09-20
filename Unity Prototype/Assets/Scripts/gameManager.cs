using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;

    [Header("UI")]
    [SerializeField] TMP_Text killCountText;
    [SerializeField] TMP_Text hostageCountText;
    [SerializeField] public TMP_Text ammoCounterText;
    [SerializeField] TMP_Text missionObjectiveText;
    [SerializeField] TMP_Text winMessageText;
    [SerializeField] TMP_Text loseMessageText;
    [SerializeField] public TMP_Text hpArmorAddedText;
    [SerializeField] public TMP_Text ammoAddedText;
    [SerializeField] public TMP_Text keyCounterText;

    [Header("Grenade UI")]
    [SerializeField] Image grenadeIconImage;
    [SerializeField] TMP_Text grenadeCountText;

    [Header("Player")]
    public bool isPaused;
    public GameObject player;
    public playerController playerScript;
    public Image playerHPBar;
    public Image playerArmorBar;
    public GameObject damageFlashPanel;

    public int totalHostages;
    public int rescuedHostages;

    float timeScaleOrig;
    int pauseInputSuppressedFrame = -1;
    int gameGoalCount;
    int killCount;

    public GameObject playerSpawnPos;
    public GameObject checkpointPopup;
    public GameObject ammoAddedPopup;
    public GameObject hpArmorAddedPopup;
    public GameObject reloadPopup;

    void Awake()
    {
        instance = this;

        player = GameObject.FindWithTag("Player");

        if (player != null)
        {
            playerScript = player.GetComponent<playerController>();
        }

        timeScaleOrig = Time.timeScale;

        if (killCountText != null)
        {
            killCountText.text = "Kills: 0";
        }

        if (hostageCountText != null)
        {
            hostageCountText.text = "Rescued: 0/0";
        }

        if (grenadeIconImage != null)
        {
            grenadeIconImage.sprite = null;
            grenadeIconImage.enabled = false;
        }

        if (grenadeCountText != null)
        {
            grenadeCountText.text = "x0";
        }

        playerSpawnPos = GameObject.FindWithTag("Player Spawn Position");

        if (playerSpawnPos == null)
        {
            Debug.LogError("PLAYER SPAWN POSITION NOT FOUND!");
        }
        else
        {
            Debug.Log("Player Spawn Position FOUND: " + playerSpawnPos.name);
        }
    }

    private void Start()
    {
        UpdateGrenadeUI();
    }

    void Update()
    {
        UpdateGrenadeUI();

        if (Input.GetButtonDown("Cancel"))
        {
            if (pauseInputSuppressedFrame == Time.frameCount)
            {
                return;
            }

            if (isPaused && menuActive == null)
            {
                return;
            }

            if (menuActive == null)
            {
                statePause();
                menuActive = menuPause;
                menuActive.SetActive(true);
            }
            else if (menuActive == menuPause)
            {
                stateUnpause();
            }
        }
    }

    public void SuppressPauseInputThisFrame()
    {
        pauseInputSuppressedFrame = Time.frameCount;
    }

    public void statePause()
    {
        isPaused = true;
        Time.timeScale = 0;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void stateUnpause()
    {
        isPaused = false;
        Time.timeScale = timeScaleOrig;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (menuActive != null)
        {
            menuActive.SetActive(false);
            menuActive = null;
        }
    }

    public void UpdateGrenadeUI()
    {
        if (grenadeIconImage == null || grenadeCountText == null)
        {
            return;
        }

        if (playerScript == null)
        {
            player = GameObject.FindWithTag("Player");

            if (player != null)
            {
                playerScript = player.GetComponent<playerController>();
            }
        }

        if (playerScript == null)
        {
            grenadeIconImage.sprite = null;
            grenadeIconImage.enabled = false;
            grenadeCountText.text = "x0";
            return;
        }

        GrenadeItemStats currentGrenade = playerScript.GetCurrentGrenade();
        int grenadeAmount = playerScript.GetCurrentGrenadeAmount();

        if (currentGrenade == null || grenadeAmount <= 0)
        {
            grenadeIconImage.sprite = null;
            grenadeIconImage.enabled = false;
            grenadeCountText.text = "x0";
            return;
        }

        grenadeIconImage.sprite = currentGrenade.grenadeIcon;
        grenadeIconImage.enabled = currentGrenade.grenadeIcon != null;
        grenadeIconImage.preserveAspect = true;

        grenadeCountText.text = "x" + grenadeAmount;
    }

    public void RegisterHostage()
    {
        totalHostages++;
        UpdateHostageUI();

        Debug.Log("Hostage registered. Total hostages: " + totalHostages);
    }

    public void hostageRescued()
    {
        rescuedHostages++;
        UpdateHostageUI();

        Debug.Log("Hostage rescued: " + rescuedHostages + "/" + totalHostages);

        if (rescuedHostages >= totalHostages)
        {
            setMissionObjective("Return to Extraction");
        }
    }

    private void UpdateHostageUI()
    {
        if (hostageCountText != null)
        {
            hostageCountText.text = "Rescued: " + rescuedHostages + " / " + totalHostages;
        }
    }

    public void addKill()
    {
        killCount++;

        if (killCountText != null)
        {
            killCountText.text = "Kills: " + killCount;
        }
    }

    public int getKillCount()
    {
        return killCount;
    }

    public void youLose()
    {
        statePause();

        menuActive = menuLose;
        menuActive.SetActive(true);
    }

    public void winGame()
    {
        statePause();

        menuActive = menuWin;
        menuActive.SetActive(true);
    }

    public void missionWin(string message)
    {
        winMessageText.text = message;
        winGame();
    }

    public void missionLose(string message)
    {
        loseMessageText.text = message;
        youLose();
    }

    public void setMissionObjective(string objective)
    {
        if (missionObjectiveText != null)
        {
            missionObjectiveText.text = objective;
        }
    }
}