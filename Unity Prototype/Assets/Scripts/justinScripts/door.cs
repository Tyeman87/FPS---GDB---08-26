using UnityEngine;

public class door : MonoBehaviour
{
    [SerializeField] GameObject model;
    [SerializeField] GameObject UI;

    bool canOpenDoor;
    public bool isLocked;

    void Update()
    {
        if (canOpenDoor)
        {
            if (Input.GetButtonDown("Interact"))
            {
                if (!isLocked)
                {
                    UI.SetActive(false);
                    model.SetActive(false);

                }
                else if (isLocked && gameManager.instance.playerScript.keyCount > 0)
                {
                    UI.SetActive(false);
                    model.SetActive(false);
                    gameManager.instance.playerScript.keyCount--;//use up 1 of player's keys
                    gameManager.instance.keyCounterText.text = $"Keys: {gameManager.instance.playerScript.keyCount}";
                    gameManager.instance.playerScript.updatePlayerUI();
                    isLocked = false;
                }
            }
        }
    }



    private void OnTriggerEnter(Collider other)
    {
        IOpen open = other.GetComponent<IOpen>();
        if (open != null)
        {
            UI.SetActive(true);
            if (isLocked)
            {
                if (gameManager.instance.playerScript.keyCount > 0)
                {
                    canOpenDoor = true;
                }
                else
                {
                    canOpenDoor = false;
                }
            }
            else
            {
                canOpenDoor = true;
            }
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
