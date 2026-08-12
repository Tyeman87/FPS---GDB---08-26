using UnityEngine;

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

    public struct DamageData
    {
        public DamageType DamageType;
        public int amount;
        public float damageRadius;
        public float damageDuration;
    }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
