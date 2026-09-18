using UnityEngine;

public class StealthGameMode : MonoBehaviour
{
    public static StealthGameMode Instance;

    public bool stealthModeActive = true;

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
}
