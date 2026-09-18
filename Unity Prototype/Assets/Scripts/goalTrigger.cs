using UnityEngine;

public class goalTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        // Check if all hostages have been rescued
        if (gameManager.instance.rescuedHostages >=
            gameManager.instance.totalHostages)
        {
            Debug.Log("All hostages rescued! Extraction successful!");

            gameManager.instance.winGame();
        }
        else
        {
            Debug.Log(
                "Cannot extract yet. Hostages rescued: " +
                gameManager.instance.rescuedHostages +
                "/" +
                gameManager.instance.totalHostages
            );
        }
    }
}