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
        

        playerSpawnPos = GameObject.FindWithTag("Player Spawn Position");

        if (playerSpawnPos == null)
        {
            Debug.LogError("PLAYER SPAWN POSITION NOT FOUND!");
        }
        else
        {
            Debug.Log("Player Spawn Position FOUND: " + playerSpawnPos.name);
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
            else if(menuActive == menuPause)
            {
                stateUnpause();
            }
        }
    }

    public void SuppressPauseInputThisFrame()
    {
        pauseInputSuppressedFrame = Time.frameCount;
    }

    private void Start()
    {
        LoadAudioSettings();
        
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


    public void SetMusicVolume(float sliderVal)
    {
        sliderVal = Mathf.Clamp(sliderVal, 0.0001f, 1f);
        float db = Mathf.Log10(sliderVal) * 20;

        mainMixer.SetFloat("musicVol", db);
        PlayerPrefs.SetFloat(MusicPrefKey, sliderVal);
    }

    public void SetSFXVolume(float sliderVal)
    {
        sliderVal = Mathf.Clamp(sliderVal, 0.0001f, 1f);
        float db = Mathf.Log10(sliderVal) * 20;

        mainMixer.SetFloat("sfxVol", db);
        PlayerPrefs.SetFloat(SFXPrefKey, sliderVal);
    }

    private void LoadAudioSettings()
    {
        float savedMusic = PlayerPrefs.GetFloat(MusicPrefKey, DefaultVolume);
        float savedSFX = PlayerPrefs.GetFloat(SFXPrefKey, DefaultVolume);

        SetMusicVolume(savedMusic);
        SetSFXVolume(savedSFX);

        if (musicSlider != null)
        {
            musicSlider.value = savedMusic;
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
        }

        if (sfxSlider != null)
        {
            sfxSlider.value = savedSFX;
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        }


    }


}