using UnityEngine;

public class hostageInteractUI : MonoBehaviour
{
    [SerializeField] GameObject interactUI;
    private hostageAI hostageScript;
    private bool playerInTrigger;

    private Transform playerCamera;

    private void Start()
    {
        playerCamera = Camera.main.transform;

        hostageScript = GetComponent<hostageAI>();
    }

    private void Update()
    {
        if (interactUI.activeSelf && playerCamera != null)
        {
            interactUI.transform.LookAt(playerCamera);
            interactUI.transform.Rotate(0f, 180f, 0f);
        }

        if (playerInTrigger && Input.GetButtonDown("Interact"))
        {
            if (hostageScript != null)
            {
                hostageScript.Interact();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = true;
            interactUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = false;
            interactUI.SetActive(false);
        }
    }
}
