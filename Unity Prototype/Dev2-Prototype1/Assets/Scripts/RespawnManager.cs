using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class RespawnManager : MonoBehaviour
{
    public static RespawnManager instance;

    [SerializeField] public float respawnTimer = 3f;

    public Transform playerSpawnPoint;
    public Transform enemySpawnPoint;
    //Just using one spawn point for now. For multiple spawn points:
    // public List<Transform> enemySpawnPoints = new List<Transform>();

    public int enemyKills = 0;

    void Awake()
    {
        if (instance == null)
        { instance = this; }
        else
        { Destroy(gameObject); }
    }


    public void HandleEnemyDeath(GameObject enemy)
    {
        Collider coll = enemy.GetComponent<Collider>();
        if (coll)
        {
            coll.enabled = false;
        }

        Rigidbody rb = enemy.GetComponent<Rigidbody>();
        if (rb)
        {
            rb.isKinematic = true;
        }

        NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
        if (agent)
        {
            agent.enabled = false;
        }

        Renderer[] renderers = enemy.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
        {
            r.enabled = false;
        }

        StartCoroutine(RespawnEnemy(enemy));
    }

    IEnumerator RespawnEnemy(GameObject enemy)
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
