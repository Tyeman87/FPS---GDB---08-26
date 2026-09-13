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
    public MissionState currentState => currentState;

    private bool missionActive;
    private bool missionComplete;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        StartMission();
    }

    public void StartMission()
    {
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

        Debug.Log($"Mission '{missionName}' completed successfully!");

        gameManager.instance.missionWin();
    }

    public void LoseMission()
    {
        if (!missionActive || missionComplete)
        {
            return;
        }

        missionComplete = true;
        missionActive = false;
        Debug.Log($"Mission '{missionName}' failed.");
    }

    public bool ShouldRespawnEnemies()
    {
        if (currentGameMode == GameMode.Assault)
        {
            return false;
        }

        return true;
    }
}
