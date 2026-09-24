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
            return null;
        }

        if (spawnPoint == null)
        {
            return null;
        }

        GameObject hostage = Instantiate(
            hostagePrefab,
            spawnPoint.position,
            spawnPoint.rotation
        );

        spawnedHostage =
            hostage.GetComponent<hostageAI>();

        if (spawnedHostage == null)
        {
            Destroy(hostage);

            return null;
        }
        return spawnedHostage;
    }

    public hostageAI GetHostage()
    {
        return spawnedHostage;
    }
}