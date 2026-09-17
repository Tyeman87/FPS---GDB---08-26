using UnityEngine;

public class ExtractionZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<playerController>() != null)
        {
            Debug.Log("PLAYER REACHED EXTRACTION!");
        }
    }
}
