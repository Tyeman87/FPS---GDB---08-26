using UnityEngine;

public class flagObject : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            

            gameObject.SetActive(false);
            
        }
    }
}
