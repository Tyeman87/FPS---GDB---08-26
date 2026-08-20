using UnityEngine;

public class highScoreManager : MonoBehaviour
{
    public static highScoreManager Instance;

    private const string HIGH_SCORE_KEY = "HighScore";

    private void Awake()
    {
        if (Instance = null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveHighScore(int score)
    {
        int currentHighScore = GetHighScore();

        if (score > currentHighScore)
        {
            PlayerPrefs.SetInt(HIGH_SCORE_KEY, score);
            PlayerPrefs.Save();
        }
    }

    public int GetHighScore()
    {
        return PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0);
    }
}
