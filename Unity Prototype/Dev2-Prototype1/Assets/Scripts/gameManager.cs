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
    [SerializeField] TMP_Text killCountText;

    public bool isPaused;
    public GameObject player;
    public playerController playerScript;
    public Image playerHPBar;
    public GameObject damageFlashPanel;
    public Image flagUI;

    public bool playerHasFlag;

    float timeScaleOrig;
    int gameGoalCount;
    int killCount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;

        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<playerController>();

        timeScaleOrig = Time.timeScale;

        killCountText.text = "Kills: 0";

        flagUI.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Cancel"))
        {
            if(menuActive == null)
            {
                // pause the game
                statePause();
                menuActive = menuPause;
                menuActive.SetActive(true);
            }
            else if(menuActive == menuPause)
            {
                // unpause the game and return to gameplay
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

    public void updateGameGoal(int amount)
    {
        gameGoalCount += amount;

        if(gameGoalCount <= 0)
        {
            // you win!!
            statePause();

            menuActive = menuWin;
            menuActive.SetActive(true);
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