using UnityEngine;
using System.Collections;



public class ammoPickup : MonoBehaviour
{
    [Range(0, 120)] [SerializeField] float reenableTimer;
    [Range(10, 300)] [SerializeField] int ammoAmount;
    

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            
            gameManager.instance.playerScript.AddReserveAmmo(ammoAmount);

            //deactivate mesh and collider for duration of timer
            GetComponent<BoxCollider>().enabled = false;
            GetComponent<MeshRenderer>().enabled = false;
            StartCoroutine(displayPopup());
            StartCoroutine(reenableCoroutine());
        }
    }

    IEnumerator reenableCoroutine()
    {
        yield return new WaitForSeconds(reenableTimer);
        GetComponent<BoxCollider>().enabled = true;
        GetComponent<MeshRenderer>().enabled = true;
    }

    IEnumerator displayPopup()
    {
        gameManager.instance.ammoAddedText.text = $"+ {ammoAmount} rounds";
        gameManager.instance.ammoAddedPopup.SetActive(true);
        yield return new WaitForSeconds(2f);
        gameManager.instance.ammoAddedPopup.SetActive(false);
    }
}
