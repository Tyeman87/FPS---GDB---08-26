using UnityEngine;

public class scoreManager : MonoBehaviour
{
    public static scoreManager Instance;

    [Header("Score Settings")]
    public int pointsPerKill = 100;
    public float timePenalty = 2f;

    private int finalScore = 0;

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

    public int CalculateScore()
    {
        int enemiesKilled = gameStats.Instance.enemiesKilled;
        float timeTaken = gameStats.Instance.timeTaken;

        finalScore = Mathf.RoundToInt((enemiesKilled * pointsPerKill) - (timeTaken * timePenalty));

        if (finalScore < 0)
        {
            finalScore = 0;
        }
        return finalScore;
    }

    public int GetFinalScore()
    {
        return finalScore;
    }
}
