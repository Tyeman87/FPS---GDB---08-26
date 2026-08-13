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
    [SerializeField] int bulletDestroyTime;
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
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;

        // Check if the other object has the InterfaceDamage component
        IDamage damageable = other.GetComponentInParent<IDamage>();

        Debug.Log("Hit: " + other.gameObject.name + " | IDamage: " + (damageable != null));

        // If it does, apply damage based on the type of damage
        if (damageable != null && type != DamageType.DOT)
        {
            damageable.takeDamage(damageAmount);
        }

        if (type == DamageType.Bullet)
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
