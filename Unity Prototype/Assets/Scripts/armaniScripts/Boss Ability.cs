using UnityEngine;

public class BossAbility : MonoBehaviour
{
    [SerializeField] GameObject drone;
    [SerializeField] Transform dronespawn;
    [SerializeField] Transform dronespawn2;
    [SerializeField] Transform dronespawn3;

    enemyAI bossAI;
    bool hasSpawnedEnemies = false;

    void Start()
    {
        bossAI = GetComponent<enemyAI>();
    }

    void Update()
    {
        if (bossAI.HP <= bossAI.maxHP * 0.5f && !hasSpawnedEnemies)
        {
            SpawnEnemies();
            hasSpawnedEnemies = true;
        }
    }

    void SpawnEnemies()
    {
        Spawndrone();
    }

    public void Spawndrone()
    {
        Instantiate(drone, dronespawn.position, dronespawn.rotation);
        Instantiate(drone, dronespawn2.position, dronespawn2.rotation);
        Instantiate(drone, dronespawn3.position, dronespawn3.rotation);
    }
}