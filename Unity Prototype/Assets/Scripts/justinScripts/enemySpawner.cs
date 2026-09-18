using UnityEngine;

public class enemySpawner : MonoBehaviour
{
    public GameObject enemyTypePrefab;
    [SerializeField] public int amountToSpawn;
    [SerializeField] int spawnTimer;

    public void SpawnEnemies()
    {
        for (int i = 0; i < amountToSpawn; i++)
        {
            Instantiate(
                enemyTypePrefab,
                transform.position,
                Quaternion.identity
            );
        }
    }
}

