using System.Collections;
using UnityEngine;

public class pickup : MonoBehaviour
{
    [Range(0,30)] [SerializeField] int healAmount;
    [Range(0, 30)] [SerializeField] int armorAmount;
    [Range(0, 120)] [SerializeField] int timer;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Item picked up. Adding " + healAmount + " HP and " + armorAmount + " armor.");
            gameManager.instance.playerScript.addHealth(healAmount);
            gameManager.instance.playerScript.addArmor(armorAmount);
            gameManager.instance.playerScript.updatePlayerUI();
            displayPopup();
            //deactivate mesh and collider for duration of timer
            GetComponent<BoxCollider>().enabled = false;
            GetComponent<MeshRenderer>().enabled = false;

            StartCoroutine(displayPopup());
            StartCoroutine(reenableTimer());
        }
    }

    IEnumerator reenableTimer()
    {
        yield return new WaitForSeconds(timer);
        GetComponent<BoxCollider>().enabled = true;
        GetComponent<MeshRenderer>().enabled = true;
    }

    IEnumerator displayPopup()
    {
        gameManager.instance.hpArmorAddedText.text = $"+{healAmount} HP  +{armorAmount} ARMOR";
        gameManager.instance.hpArmorAddedPopup.SetActive(true);
        yield return new WaitForSeconds(2);
        gameManager.instance.hpArmorAddedPopup.SetActive(false);
    }


}
