using UnityEngine;

public class HostageSpawner : MonoBehaviour
{
    [Header("Hostage")]
    [SerializeField] private GameObject hostagePrefab;
    [SerializeField] private Transform spawnPoint;

    private hostageAI spawnedHostage;

    private void Start()
    {
        SpawnHostage();
    }

    public hostageAI SpawnHostage()
    {
        if (hostagePrefab == null)
        {
            Debug.LogError("HostageSpawner: No hostage prefab assigned!");
            return null;
        }

        if (spawnPoint == null)
        {
            Debug.LogError("HostageSpawner: No spawn point assigned!");
            return null;
        }

        // Spawn the hostage
        GameObject hostage = Instantiate(
            hostagePrefab,
            spawnPoint.position,
            spawnPoint.rotation
        );

        // Get hostage AI
        spawnedHostage = hostage.GetComponent<hostageAI>();

        if (spawnedHostage == null)
        {
            Debug.LogError(
                "Hostage prefab does not have a hostageAI component!"
            );

            return null;
        }

        Debug.Log("Hostage spawned!");

        return spawnedHostage;
    }

    public hostageAI GetHostage()
    {
        return spawnedHostage;
    }
}