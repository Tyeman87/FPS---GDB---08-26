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

        //reset enemy position to enemySpawnpoint object
        enemy.transform.position = enemySpawnPoint.position;

        //reenable physics and ai
        Collider coll = enemy.GetComponent<Collider>();
        if (coll)
        {
            coll.enabled = true;
        }

        Rigidbody rb = enemy.GetComponent<Rigidbody>();
        if (rb)
        {
            rb.isKinematic = false;
        }

        NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
        if (agent)
        {
            agent.enabled = true;
        }

        Renderer[] renderers = enemy.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
        {
            r.enabled = true;
        }

        //reset enemy health and color
        enemyAI aiScript = enemy.GetComponent<enemyAI>();
        if (aiScript != null)
        {
            aiScript.HP = aiScript.maxHP;
            aiScript.model.material.color = aiScript.colorOrig;//makes sure enemy doesn't stay red when they die
        }

        if (gameStats.Instance != null)
        {
            gameStats.Instance.EnemySpawned();
        }
    }

}
