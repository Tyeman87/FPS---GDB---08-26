using UnityEngine;

[CreateAssetMenu(
    fileName = "New Grenade Item",
    menuName = "Items/Grenade Item"
)]
public class GrenadeItemStats : ItemStats
{

    [Header("Visuals")]
    public GameObject grenadeModel;

    public GameObject grenadePrefab;

    public Sprite grenadeIcon;

    [Header("Grenade Stats")]
    [Min(0)]
    public int damage = 75;

    [Min(0.1f)]
    public float blastRadius = 5f;

    [Tooltip("Maximum distance the grenade will be aimed toward.")]
    [Min(1f)]
    public float throwRange = 15f;
}