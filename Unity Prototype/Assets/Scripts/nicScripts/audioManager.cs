using UnityEngine;

public class audioManager : MonoBehaviour
{
    public static audioManager Instance;
    public AudioSource audPlayer;

    private void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (Time.timeScale == 0)
        {
            audPlayer.Pause();
        }
        else 
            audPlayer.UnPause();
    }
}
