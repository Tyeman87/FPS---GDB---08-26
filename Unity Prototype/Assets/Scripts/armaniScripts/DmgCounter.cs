using TMPro;
using UnityEngine;

public class DmgCounter : MonoBehaviour
{
    public static DmgCounter instance;

    [SerializeField] TextMeshProUGUI Dmgtxt;
    [SerializeField] GameObject DmgUI;

    [SerializeField] float resetTime = 1.3f;

    int totalDamage;
    float dmgTimer;

    private void Awake()
    {
        instance = this;

        DmgUI.SetActive(false);
    }

    private void Update()
    {
        if (DmgUI.activeSelf)
        {
            dmgTimer -= Time.deltaTime;

            if (dmgTimer <= 0)
            {
                resetDmg();
            }
        }
    }

    public void addDmg(int amount)
    {
        totalDamage += amount;

        Dmgtxt.text = "DMG: " + totalDamage;

        DmgUI.SetActive(true);

        dmgTimer = resetTime;
    }

    public void resetDmg()
    {
        totalDamage = 0;

        Dmgtxt.text = "DMG: 0";

        DmgUI.SetActive(false);
    }
}