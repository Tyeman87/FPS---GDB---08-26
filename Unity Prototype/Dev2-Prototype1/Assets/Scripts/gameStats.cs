using UnityEngine;

public class gameStats : MonoBehaviour
{
    public static gameStats Instance;

    public float timeTaken = 0f;
    public int enemiesKilled = 0;

    private bool gameStarted = false;
    private bool gameFinished = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (gameStarted && !gameFinished)
        {
            timeTaken += Time.deltaTime;
        }
    }
}
