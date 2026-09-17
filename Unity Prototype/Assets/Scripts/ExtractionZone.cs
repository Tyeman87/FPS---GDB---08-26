using UnityEngine;

public class ExtractionZone : MonoBehaviour
{
    [SerializeField] private hostageAI hostage;
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<playerController>() != null)
        {
            if (hostage != null && hostage.IsRescued())
            {
                Debug.Log("PLAYER REACHED EXTRACTION WITH HOSTAGE!");
            }
            else
            {
                Debug.Log("You must rescue the hostage first!");
            }
        }
    }
}
