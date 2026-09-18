using UnityEngine;
using System.Collections;
using static damage;

public class damage : MonoBehaviour
{
    public enum DamageType
    {
        Bullet,
        Stationary,
        DOT,
        Spread,
        Melee,
        Piercing,
    }

    [SerializeField] DamageType type;
    [SerializeField] Rigidbody rb;

    [SerializeField] int damageAmount;
    [SerializeField] int damageRate;
    [SerializeField] int bulletSpeed;
    [Range (0, 1) ] [SerializeField] float bulletDestroyTime;
    [SerializeField] ParticleSystem hitEffect;

    bool isDamaging;

    // Damage setter for different enemy types
    public void setDamage(int amount)
    {
        damageAmount = amount;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (type == DamageType.Bullet)
        {
            rb.linearVelocity = transform.forward * bulletSpeed;
            Destroy(gameObject, bulletDestroyTime);
        }
        else if(type == DamageType.Spread)
        {
            rb.linearVelocity = transform.forward * bulletSpeed;
            Destroy(gameObject, bulletDestroyTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;

        enemyAI enemy = other.GetComponentInParent<enemyAI>();

        if (enemy != null)
        {
            enemy.takeDamage(damageAmount);
        }
        else
        {
            IDamage damageable = other.GetComponentInParent<IDamage>();

            if (damageable != null)
            {
                damageable.takeDamage(damageAmount);
            }
            else
            {
            }
        }

        if (type == DamageType.Bullet || type == DamageType.Spread)
        {
            if (hitEffect != null)
            {
                Instantiate(hitEffect, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }

        if (type == DamageType.Spread)
        {
            if (hitEffect != null)
            {
                Instantiate(hitEffect, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.isTrigger)
            return;

        IDamage damageable = other.GetComponent<IDamage>();

        if (damageable != null && type == DamageType.DOT && !isDamaging)
        {
            StartCoroutine(damageOther(damageable));
        }
    }

    IEnumerator damageOther(IDamage d)
    {
        isDamaging = true;
        d.takeDamage(damageAmount);
        yield return new WaitForSeconds(damageRate);
        isDamaging = false;
    }
}
