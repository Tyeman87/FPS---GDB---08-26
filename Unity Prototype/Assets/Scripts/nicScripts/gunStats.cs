using UnityEngine;

[CreateAssetMenu]
public class GunStats : ItemStats
{
    [Header("Visuals")]
    public GameObject gunModel;
    public GameObject iconModel;

    [Header("Base Stats")]
    [Range(1, 10)]
    public int shootDamage;

    [Range(3, 1000)]
    public int shootDist;

    [Range(0.1f, 2)]
    public float shootRate;

    [Header("Ammo Capacities")]
    [Range(5, 50)]
    public int magSize;

    [Range(50, 300)]
    public int maxReserve;

    [Header("Weapon Icon")]
    public Sprite weaponIcon;

    [Header("Audio & FX")]
    public ParticleSystem hitEffect;
    public AudioClip[] shootSound;

    [Range(0, 1)]
    public float shootSoundVol;

    [Header("Upgrade Limits")]
    public int maxDamage;
    public float minShootRate;
    public int maxMagSize;

    [Header("Upgrade Amounts")]
    public int damageUpgradeAmount = 1;
    public float shootRateUpgradeAmount = 0.05f;
    public int magSizeUpgradeAmount = 1;
}