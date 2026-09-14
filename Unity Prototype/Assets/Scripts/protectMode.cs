using UnityEngine;

public class protectMode : MonoBehaviour
{
    [Header("Wave Settings")]
    [SerializeField] int totalWaves = 3;
    [SerializeField] float timeBetweenWaves = 10f;

    private int currentWave = 0;
    private int enemiesRemaining = 0;
    private float waveTimer;

    private spawner[] levelSpawners;

    private bool waitingForNextWave = false;

    private void Start()
    {
        levelSpawners = FindObjectsByType<spawner>();

        StartNextWave();
    }

    private void Update()
    {
        if (waitingForNextWave)
        {
            waveTimer -= Time.deltaTime;

            gameManager.instance.setMissionObjective(
                "WAVE " + currentWave + " COMPLETE\n" +
                "NEXT WAVE IN: " + Mathf.Ceil(waveTimer)
            );

            if (waveTimer <= 0)
            {
                StartNextWave();
            }
        }
    }

    private void StartNextWave()
    {
        currentWave++;
        enemiesRemaining = 0;
        waitingForNextWave = false;

        Debug.Log("Starting Protect Wave " + currentWave);

        foreach (spawner spawner in levelSpawners)
        {
            if (spawner != null)
            {
                enemiesRemaining += spawner.spawnAmount;
                spawner.ResetSpawner();
            }
        }

        gameManager.instance.setMissionObjective(
            "PROTECT THE OBJECTIVE\n" +
            "WAVE " + currentWave + " / " + totalWaves + "\n" +
            enemiesRemaining + " ENEMIES REMAINING"
        );
    }

    public void enemyDefeated()
    {
        if (waitingForNextWave)
        {
            return;
        }

        enemiesRemaining--;

        Debug.Log("Protect enemy defeated. Enemies Remaining: " + enemiesRemaining);

        gameManager.instance.setMissionObjective(
            "PROTECT THE OBJECTIVE\n" +
            "WAVE " + currentWave + " / " + totalWaves + "\n" +
            enemiesRemaining + " ENEMIES REMAINING"
        );

        if (enemiesRemaining <= 0)
        {
            if (currentWave >= totalWaves)
            {
                Debug.Log("All Protect Waves Complete!");

                missionManager.instance.WinMission();
            }
            else
            {
                waitingForNextWave = true;
                waveTimer = timeBetweenWaves;

                Debug.Log(
                    "Protect Wave " + currentWave +
                    " Complete! Next wave in " +
                    timeBetweenWaves + " seconds."
                );
            }
        }
    }
}
