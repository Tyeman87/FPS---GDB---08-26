using UnityEngine;

public class protectMode : MonoBehaviour
{
    [SerializeField] float survivalTime = 60f;

    private float timer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        timer = survivalTime;

        Debug.Log("Protect Mission Started. Survive for " + survivalTime + " seconds.");

        gameManager.instance.setMissionObjective(
        "PROTECT THE OBJECTIVE\n" +
        "SURVIVE: " + Mathf.CeilToInt(timer) + " SECONDS"
        );
    }

    // Update is called once per frame
    private void Update()
    {
        if (missionManager.instance == null)
        {
            return;
        }

        if (missionManager.instance.CurrentState != missionManager.MissionState.Active)
        {
            return;
        }

        timer -= Time.deltaTime;

        gameManager.instance.setMissionObjective(
        "PROTECT THE OBJECTIVE\n" +
        "SURVIVE: " + Mathf.CeilToInt(timer) + " SECONDS"
        );

        if (timer < 0)
        {
            timer = 0;

            Debug.Log("Protect Mission Survived!");

            missionManager.instance.WinMission();
        }
    }
}
