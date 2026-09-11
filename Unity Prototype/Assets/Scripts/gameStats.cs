using UnityEngine;

public class gameStats : MonoBehaviour
{
    public static gameStats Instance;

    public float timeTaken = 0f;
    public int enemiesKilled = 0;
    public int enemiesAlive = 0;

    private bool gameStarted = false;
    private bool gameFinished = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        enemiesAlive = enemies.Length;

        Debug.Log("Enemies Alive: " + enemiesAlive);
    }

    private void Update()
    {
        if (gameStarted && !gameFinished)
        {
            timeTaken += Time.deltaTime;
        }
    }

    public void StartGame()
    {
        timeTaken = 0f;
        enemiesKilled = 0;

        gameStarted = true;
        gameFinished = false;
    }

    public void EnemyKilled()
    {
        enemiesKilled++;
        enemiesAlive--;
    }

    public void enemySpawned()
    {
        enemiesAlive++;
    }

    public void FinishGame()
    {
        gameFinished = true;
        gameStarted = false;
    }

    public void ResetStats()
    {
        timeTaken = 0f;
        enemiesKilled = 0;

        gameStarted = false;
        gameFinished = false;
    }
}
