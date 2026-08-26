using UnityEngine;
using TMPro;

public class gameStatsUI : MonoBehaviour
{
    public TMP_Text statsText;

    private void Update()
    {
        if (gameStats.Instance == null)
            return;

        float time = gameStats.Instance.timeTaken;
        int kills = gameStats.Instance.enemiesKilled;
        int enemiesAlive = gameStats.Instance.enemiesAlive;

        int score = 0;

        if (scoreManager.Instance != null)
        {
            score = scoreManager.Instance.CalculateScore();
        }

        int highScore = 0;

        if (highScoreManager.Instance != null)
        {
            highScore = highScoreManager.Instance.GetHighScore();
        }

        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);

        statsText.text = "Time: " + minutes.ToString("00") + ":" + seconds.ToString("00") + "\n" + "Enemies Alive: " + enemiesAlive + "\n" + "Enemies Killed: " + kills + "\n" + "Score: " + score + "\n" + "High Score: " + highScore;
    }
}
