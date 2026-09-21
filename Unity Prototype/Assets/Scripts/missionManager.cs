using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

public class missionManager : MonoBehaviour
{
    public static missionManager instance;

    public enum GameMode
    {
        Protect,
        Assault,
        Hostage,
        Stealth
    }

    public enum MissionState
    {
        NotStarted,
        Active,
        Completed,
        Lost
    }

    [Header("Mission Settings")]
    [SerializeField] string missionName;
    [SerializeField] GameMode currentGameMode;
    [SerializeField] public int rewardMoney = 500;
    //[SerializeField] string lobbyRoomName = "LobbyRoom";
    private MissionState currentState = MissionState.NotStarted;
    public MissionState CurrentState => currentState;

    private bool missionActive;
    private bool missionComplete;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        DetectGameMode();
        StartMission();
    }

    public void StartMission()
    {
        currentState = MissionState.Active;

        missionActive = true;
        missionComplete = false;

        Debug.Log($"Mission '{missionName}' started in {currentGameMode} mode.");
    }

    public void WinMission()
    {
        if (!missionActive || missionComplete)
        {
            return;
        }

        missionComplete = true;
        missionActive = false;
        currentState = MissionState.Completed;

        if (SaveDataManager.Instance != null)
        {
            SaveDataManager.Instance.AddCredits(rewardMoney);
        }

        string message = "";

        if (currentGameMode == GameMode.Hostage)
        {
            message = "HOSTAGES RESCUED!";
        }
        else if (currentGameMode == GameMode.Assault)
        {
            message = "AREA SECURED!";
        }
        else if (currentGameMode == GameMode.Protect)
        {
            message = "OBJECTIVE PROTECTED!";
        }
        else if (currentGameMode == GameMode.Stealth)
        {
            message = "HOSTAGE EXTRACTED";
        }

        Debug.Log($"Mission '{missionName}' completed successfully!");

        gameManager.instance.missionWin(message);
    }

    public void LoseMission(string message)
    {
        if (!missionActive || missionComplete)
        {
            return;
        }

        missionComplete = true;
        missionActive = false;
        currentState = MissionState.Lost;

        Debug.Log($"Mission '{missionName}' failed.");

        gameManager.instance.missionLose(message);
    }


    public bool ShouldRespawnEnemies()
    {
        if (currentGameMode == GameMode.Assault ||
        currentGameMode == GameMode.Protect)
        {
            return false;
        }

        return true;
    }

    private void DetectGameMode()
    {
        if (FindAnyObjectByType<hostageMode>() != null)
        {
            currentGameMode = GameMode.Hostage;
        }
        else if (FindAnyObjectByType<assaultMode>() != null)
        {
            currentGameMode = GameMode.Assault;
        }
        else if (FindAnyObjectByType<protectMode>() != null)
        {
            currentGameMode = GameMode.Protect;
        }
        else if (FindAnyObjectByType<StealthGameMode>() != null)
        {
            currentGameMode = GameMode.Stealth;
        }
        else
        {
            Debug.LogWarning("No game mode object found in this scene!");
        }

        Debug.Log("Detected Game Mode: " + currentGameMode);
    }

    public bool IsStealthMode()
    {
        return currentGameMode == GameMode.Stealth;
    }
}
