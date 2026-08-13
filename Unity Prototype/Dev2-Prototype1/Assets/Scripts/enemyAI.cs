using System.Collections;
using UnityEngine.AI;
using UnityEngine;

public class enemyAI : MonoBehaviour, IDamage
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] public Renderer model;

    [Header("Stats")]
    [Range(1, 30)] [SerializeField] public int HP;
    [Range(1, 30)] [SerializeField] public int maxHP;


    public Color colorOrig;
    bool isDead = false; 

    void Start()
    {
        colorOrig = model.material.color;
        HP = maxHP;
        gameManager.instance.updateGameGoal(1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void takeDamage(int amount)
    {
        if (isDead) return;

        HP -= amount;

        if(HP <= 0)
        {
            isDead = true;

            RespawnManager.instance.HandleEnemyDeath(gameObject);
            //gameManager.instance.updateGameGoal(-1);
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
