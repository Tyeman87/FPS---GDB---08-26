using UnityEngine;

public class goalTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Object.FindAnyObjectByType <levelManager>().WinLevel();
        }
    }
}