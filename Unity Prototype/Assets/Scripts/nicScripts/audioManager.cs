using UnityEngine;

public class audioManager : MonoBehaviour
{
    public static audioManager Instance;
    public AudioSource audPlayer;

    private void Awake()
    {
        Instance = this;
    }
}
