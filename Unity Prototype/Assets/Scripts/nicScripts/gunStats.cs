using UnityEngine;

[CreateAssetMenu]

public class GunStats : ItemStats
{
    public GameObject gunModel;

    [Range(1, 10)] public int shootDamage;
    [Range(3, 1000)] public int shootDist;
    [Range(0.1f, 2)] public float shootRate;

    [Header("Ammo capacities")]
    [Range(5, 50)] public int magSize;
    [Range(50, 300)] public int maxReserve;

    [Header("Audio & FX")]
    public ParticleSystem hitEffect;
    public AudioClip[] shootSound;
    [Range(0, 1)] public float shootSoundVol;
}
