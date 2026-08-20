using UnityEngine;
using UnityEngine.SceneManagement;

public class levelManager : MonoBehaviour
{
    public static int currentLevel = 1;
    public static float difficultyMultiplier = 1.0f;
    public static float difficultyIncreaseRate = 0.2f;

    private void Start()
    {
        gameStats.Instance.StartGame();
    }

    public void WinLevel()
    {
        gameStats.Instance.FinishGame();

        int score = scoreManager.Instance.CalculateScore();

        highScoreManager.Instance.SaveHighScore(score);

        Debug.Log("Level Completed!");
        Debug.Log("Time: " + gameStats.Instance.timeTaken);
        Debug.Log("Enemies Killed: " + gameStats.Instance.enemiesKilled);
        Debug.Log("Final Score: " + score);
        Debug.Log("High Score: " + highScoreManager.Instance.GetHighScore());

        currentLevel++;
        difficultyMultiplier += difficultyIncreaseRate;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
