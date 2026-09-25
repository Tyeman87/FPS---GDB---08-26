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
            
            StartCoroutine(displayPopup());
            gameManager.instance.playerScript.FillAmmo(ammoAmount);

            //deactivate mesh and collider for duration of timer
            GetComponent<BoxCollider>().enabled = false;
            GetComponent<MeshRenderer>().enabled = false;
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
        int amtAdded = gameManager.instance.playerScript.GetAmmoAmountToAdd();
        gameManager.instance.ammoAddedText.text = $"+{amtAdded} Rounds";
        gameManager.instance.ammoAddedPopup.SetActive(true);
        yield return new WaitForSeconds(2f);
        gameManager.instance.ammoAddedPopup.SetActive(false);
    }
}
