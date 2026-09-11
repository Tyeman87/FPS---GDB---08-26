using UnityEngine;
using System.Collections;


public class ItemDamage : MonoBehaviour, IDamage
{
    
    [SerializeField] int HP;
    [SerializeField] Renderer model;

    Color ItemDMGColor;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ItemDMGColor = model.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void takeDamage(int amount)
    {
        HP -= amount;

        if (HP <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            StartCoroutine(ItemDMGblink());
        }
    }

    IEnumerator ItemDMGblink()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = ItemDMGColor;
    }
}
