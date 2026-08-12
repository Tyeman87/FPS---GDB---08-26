using System.Collections;
using UnityEngine.AI;
using UnityEngine;

public class enemyAI : MonoBehaviour
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Renderer model;

    [Header("Stats")]
    [Range(1, 10)] [SerializeField] int HP;

    Color colorOrig;
    void Start()
    {
        colorOrig = model.material.color;
        gameManager.instance.updateGameGoal(1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void takeDamage(int amount)
    {
        HP -= amount;

        if(HP <= 0)
        {
            gameManager.instance.updateGameGoal(-1);
            Destroy(gameObject);
        }
        else
        {
            StartCoroutine(flashRed());
        }
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
    }
}
