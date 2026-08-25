using UnityEngine;

public class gunPickup : MonoBehaviour
{
    [SerializeField] gunStats gun;

    private void OnTriggerEnter(Collider other)
    {
        IPickupGun pickup = other.GetComponent<IPickupGun>();
        if (pickup != null)
        {
            gun.ammoCur = gun.ammoMax;
            pickup.getGunStats(gun);
            Destroy(gameObject);
        }
    }
}
