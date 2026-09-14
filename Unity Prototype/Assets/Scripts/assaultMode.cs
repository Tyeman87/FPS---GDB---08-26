using UnityEngine;

public class assaultMode : MonoBehaviour
{
    private int enemiesRemaining;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        enemiesRemaining = FindObjectsByType<enemyAI>().Length;

        Debug.Log("Assault Mission Started. Enemies Remaining: " + enemiesRemaining);

        gameManager.instance.setMissionObjective(
        "ELIMINATE ALL ENEMIES\n" +
        enemiesRemaining + " ENEMIES REMAINING"
    );
    }

    // Update is called once per frame
     public void enemyDefeated()
    {
        enemiesRemaining--;

        Debug.Log("Enemy defeated. Enemies Remaining: " + enemiesRemaining);

        gameManager.instance.setMissionObjective(
            "ELIMINATE ALL ENEMIES\n" +
            enemiesRemaining + " ENEMIES REMAINING"
        );

        if (enemiesRemaining <= 0)
        {
            missionManager.instance.WinMission();
        }
    }
}
