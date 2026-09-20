using UnityEngine;

public class keyPickup : MonoBehaviour
{

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.instance.playerScript.keyCount++;
            gameManager.instance.keyCounterText.text = $"Keys: {gameManager.instance.playerScript.keyCount}";
            gameManager.instance.playerScript.updatePlayerUI();
            Destroy(gameObject);
        }
    }
}
