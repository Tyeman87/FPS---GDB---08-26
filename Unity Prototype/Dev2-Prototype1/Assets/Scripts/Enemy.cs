using System.Xml.Serialization;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float baseSpeed = 2f;
    public float baseHealth = 100f;
    public float baseDamage = 10f;

    public float speed;
    public float health;
    public float damage;

    public float speedIncrease = 0.25f;
    public float healthIncrease = 20f;
    public float damageIncrease = 5f;

    private void ApplyLevelDifficulty()
    {
        int level = levelManager.currentLevel;

        speed = baseSpeed + ((level - 1) * speedIncrease);
        health = baseHealth + ((level - 1) * healthIncrease);
        damage = baseDamage + ((level - 1) * damageIncrease);
    }

    private void Start()
    {
        ApplyLevelDifficulty();
    }
}
