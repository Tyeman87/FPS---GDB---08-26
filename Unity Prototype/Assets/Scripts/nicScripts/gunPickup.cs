using UnityEngine;

public class gunPickup : MonoBehaviour
{
    [SerializeField] gunStats gun;
    [Range(0, 100)] [SerializeField] float roationSpeed = 100f;
    [SerializeField] float bobHeight = 0.25f;
    [SerializeField] float bobSpeed = 2f;

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        transform.Rotate(0f, roationSpeed * Time.deltaTime, 0f);
        float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;

        transform.position = new Vector3(
            startPosition.x,
            newY,
            startPosition.z
        );
    }

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
