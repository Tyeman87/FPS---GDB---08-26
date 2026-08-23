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

    [Header("Player")]
    public bool isPaused;
    public GameObject player;
    public playerController playerScript;
    public Image playerHPBar;
    public GameObject damageFlashPanel;

    public Image flagUI;
    public bool playerHasFlag;
    
    public int totalHostages;
    public int rescuedHostages;

    float timeScaleOrig;
    int gameGoalCount;
    int killCount;

    public GameObject playerSpawnPos;
    public GameObject checkpointPopup;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;

        player = GameObject.FindWithTag("Player");

        if (player != null)
        {
            playerScript = player.GetComponent<playerController>();
        }

        timeScaleOrig = Time.timeScale;

        killCountText.text = "Kills: 0";

        flagUI.gameObject.SetActive(false);

        playerSpawnPos = GameObject.FindWithTag("Player Spawn Position");

        if (playerSpawnPos == null)
        {
            Debug.LogError("PLAYER SPAWN POSITION NOT FOUND!");
        }
        else
        {
            Debug.Log("Player Spawn Position FOUND: " + playerSpawnPos.name);
        if (flagUI != null)
        {
            flagUI.gameObject.SetActive(false);
        }

        if (hostageCountText != null)
        {
            hostageCountText.text = "Rescued: 0/0";
        }
    }

    void Update()
    {
        if(Input.GetButtonDown("Cancel"))
        {
            if(menuActive == null)
            {
                statePause();
                menuActive = menuPause;
                menuActive.SetActive(true);
            }
            else if(menuActive == menuPause)
            {
                stateUnpause();
            }
        }
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

        menuActive.SetActive(false);
        menuActive = null;
    }

    public void RegisterHostage()
    {
        totalHostages++;
        UpdateHostageUI();
        Debug.Log(
            "Hostage registered. Total hostages: " +
            totalHostages
        );
    }

    public void hostageRescued()
    {
        rescuedHostages++;
        UpdateHostageUI();

        Debug.Log(
            "Hostage rescued: " +
            rescuedHostages + 
            "/" +
            totalHostages
        );
    }
    
    private void UpdateHostageUI()
    {
        if (hostageCountText != null)
        {
            hostageCountText.text = 
            "Rescued: " +
            rescuedHostages +
            " / " +
            totalHostages;
        }
    }

    public void addKill()
    {
        killCount++;

        killCountText.text = "Kills: " + killCount;
    }

    public int getKillCount()
    {
        return killCount;
    }

    public void playerPickedUpFlag()
    {
        playerHasFlag = true;
        flagUI.gameObject.SetActive(true);
    }

    public void playerDroppedFlag()
    {
        playerHasFlag = false;
        flagUI.gameObject.SetActive(false);
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
}