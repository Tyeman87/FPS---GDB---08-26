using UnityEngine;

public class door : MonoBehaviour
{
    [SerializeField] GameObject model;
    [SerializeField] GameObject UI;

    public AudioClip[] doorOpenSound;
    public AudioClip[] doorShutSound;
    public AudioClip[] doorLockedSound;



    bool canOpenDoor;
    public bool isLocked;

    private int entitiesInTrigger = 0;

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
                    if (doorOpenSound != null && doorOpenSound.Length > 0)
                    {
                        audioManager.Instance.audPlayer.PlayOneShot(doorOpenSound[Random.Range(0, doorOpenSound.Length)]);
                    }


                }
                else if (isLocked && gameManager.instance.playerScript.keyCount > 0)
                {
                    UI.SetActive(false);
                    model.SetActive(false);
                    gameManager.instance.playerScript.keyCount--;//use up 1 of player's keys
                    gameManager.instance.keyCounterText.text = $"Keys: {gameManager.instance.playerScript.keyCount}";
                    if (doorOpenSound != null && doorOpenSound.Length > 0)
                    {
                        audioManager.Instance.audPlayer.PlayOneShot(doorOpenSound[Random.Range(0, doorOpenSound.Length)]);
                    }
                    gameManager.instance.playerScript.updatePlayerUI();
                    isLocked = false;
                }
                else
                {
                    if (doorLockedSound != null && doorLockedSound.Length > 0)
                    {
                        audioManager.Instance.audPlayer.PlayOneShot(doorLockedSound[Random.Range(0, doorLockedSound.Length)]);
                    }

                }
            }
        }
    }



    private void OnTriggerEnter(Collider other)
    {
        bool isPlayer = other.GetComponent<IOpen>() != null;
        bool isHostage = other.GetComponent<hostageAI>() != null || other.GetComponentInParent<hostageAI>() != null;

        if (isPlayer || isHostage)
        {
            entitiesInTrigger++;

            if (isPlayer)
            {
                canOpenDoor = true;
                if (model.activeSelf)
                {
                    UI.SetActive(true);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        bool isPlayer = other.GetComponent<IOpen>() != null;
        bool isHostage = other.GetComponent<hostageAI>() != null || other.GetComponentInParent<hostageAI>() != null;

        if (isPlayer || isHostage)
        {
            entitiesInTrigger = Mathf.Max(0, entitiesInTrigger - 1);

            if (isPlayer)
            {
                {
                    canOpenDoor = false;
                    UI.SetActive(false);
                }

                if (entitiesInTrigger == 0)
                {
                    bool wasOpen = !model.activeSelf;
                    model.SetActive(true);

                    if (wasOpen && doorShutSound != null && doorShutSound.Length > 0)
                    {
                        audioManager.Instance.audPlayer.PlayOneShot(doorShutSound[Random.Range(0, doorShutSound.Length)]);
                    }
                }
            }
        }
    }
}
