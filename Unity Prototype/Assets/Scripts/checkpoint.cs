using UnityEngine;
using System.Collections;

public class checkpoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && gameManager.instance.playerSpawnPos.transform.position != transform.position)
        {
            gameManager.instance.playerSpawnPos.transform.position = transform.position;
            StartCoroutine(displayPopup());
        }
    }

    IEnumerator displayPopup()
    {
        gameManager.instance.checkpointPopup.SetActive(true);
        yield return new WaitForSeconds(2);
        gameManager.instance.checkpointPopup.SetActive(false);
    }
}
