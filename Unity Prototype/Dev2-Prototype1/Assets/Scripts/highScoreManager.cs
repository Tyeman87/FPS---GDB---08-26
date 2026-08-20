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
}
