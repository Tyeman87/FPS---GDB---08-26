using UnityEngine;

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
        if (currentGameMode == GameMode.Assault)
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
        else
        {
            Debug.LogWarning("No game mode object found in this scene!");
        }

        Debug.Log("Detected Game Mode: " + currentGameMode);
    }
}
