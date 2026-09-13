using UnityEngine;

public class hostageMode : MonoBehaviour
{
    private void Start()
    {
        gameManager.instance.setMissionObjective("Rescue the Hostages");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (gameManager.instance.rescuedHostages >=
            gameManager.instance.totalHostages)
        {
            Debug.Log("All hostages rescued! Extraction successful!");

            missionManager.instance.WinMission();
        }
        else
        {
            Debug.Log("Cannot extract yet. Hostages rescued: " + 
            gameManager.instance.rescuedHostages + "/" + gameManager.instance.totalHostages);
        }
    }
}
