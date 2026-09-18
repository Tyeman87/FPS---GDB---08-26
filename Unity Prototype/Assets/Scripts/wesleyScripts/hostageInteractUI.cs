using UnityEngine;

public class hostageInteractUI : MonoBehaviour
{
    [SerializeField] GameObject interactUI;

    private Transform playerCamera;

    private void Start()
    {
        playerCamera = Camera.main.transform;
    }

    private void Update()
    {
        if (interactUI.activeSelf && playerCamera != null)
        {
            interactUI.transform.LookAt(playerCamera);
            interactUI.transform.Rotate(0f, 180f, 0f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            interactUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            interactUI.SetActive(false);
        }
    }
}
