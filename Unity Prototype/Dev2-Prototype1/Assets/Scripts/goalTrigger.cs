using UnityEngine;

public class goalTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(gameManager.instance.playerHasFlag)
            {
                gameManager.instance.winGame();
            }
        }
    }
}