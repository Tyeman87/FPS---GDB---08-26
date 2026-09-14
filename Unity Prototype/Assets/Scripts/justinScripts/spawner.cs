using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.AI;


/* 
 IMPORTANT:
    - Not yet complete, do not use
 */
public class spawner : MonoBehaviour
{
    [SerializeField] GameObject objectToSpawn;
    [SerializeField] int spawnAmount;
    [SerializeField] int spawnDelay;

    int spawnCount;
    float spawnTimer;

    bool spawning;

    Vector3 spawnPos;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            spawning = true;
            Debug.Log("Spawner trigger entered");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (spawning)
        {
            spawnTimer += Time.deltaTime;
            if (spawnCount < spawnAmount && spawnTimer > spawnDelay)
            {
                spawn();
            }
        }
    }

    void spawn()
    {
        spawnTimer = 0;
        spawnCount++;

        spawnPos = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);


        Instantiate(objectToSpawn, spawnPos, Quaternion.Euler(0, Random.Range(0, 360), 0));
    }
}
