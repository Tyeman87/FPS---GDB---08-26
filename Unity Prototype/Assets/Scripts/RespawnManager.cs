using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class RespawnManager : MonoBehaviour
{
    public static RespawnManager instance;
    [SerializeField] public float respawnTimer = 3f;
    private enemySpawner[] levelSpawners;





    void Awake()
    {
        if (instance == null)
        { instance = this; }
        else
        { Destroy(gameObject); }

        levelSpawners = FindObjectsByType<enemySpawner>();

    }


    public void HandleEnemyDeath(GameObject enemy)
    {
        enemySpawner matchedSpawner = null;
        //compare to each spawner in level
        foreach (enemySpawner spawner in levelSpawners)
        {
            //if there is a spawner AND the enemy's name matches with spawner's prefab's name.
            if (spawner != null && enemy.name.StartsWith(spawner.enemyTypePrefab.name))
            {
                matchedSpawner = spawner;
                break;
            }
        }

        if (matchedSpawner != null)
        {
            Debug.Log("Starting respawn coroutine");
            StartCoroutine(RespawnCoroutine(matchedSpawner.enemyTypePrefab, matchedSpawner.transform));
        }

        Debug.Log("Destroying Enemy");
        Destroy(enemy);
    }


    IEnumerator RespawnCoroutine(GameObject prefab, Transform spawner)
    {
        yield return new WaitForSeconds(respawnTimer);
        Debug.Log("Respawning Enemy:" + prefab.name);
        Instantiate(prefab, spawner.position, Quaternion.identity);
    }

}
