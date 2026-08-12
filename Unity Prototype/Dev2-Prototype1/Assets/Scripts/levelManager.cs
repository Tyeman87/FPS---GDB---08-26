using UnityEngine;
using UnityEngine.SceneManagement;

public class levelManager : MonoBehaviour
{
    public static int currentLevel = 1;
    public static float difficultyMultiplier = 1.0f;
    public static float difficultyIncreaseRate = 0.2f;

    public void WinLevel()
    {
        currentLevel++;
        difficultyMultiplier += difficultyIncreaseRate;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
