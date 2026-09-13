using UnityEngine;

public class door : MonoBehaviour
{
    [SerializeField] GameObject model;
    [SerializeField] GameObject UI;

    bool canOpenDoor;

    void Update()
    {
        if (canOpenDoor)
        {
            if (Input.GetButtonDown("Interact"))
            {
                UI.SetActive(false);
                model.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        IOpen open = other.GetComponent<IOpen>();
        if (open != null)
        {
            UI.SetActive(true);
            canOpenDoor = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        IOpen open = other.GetComponent<IOpen>();
        if (open != null)
        {
            model.SetActive(true);
            UI.SetActive(false);
            canOpenDoor = false;
        }
    }
}
