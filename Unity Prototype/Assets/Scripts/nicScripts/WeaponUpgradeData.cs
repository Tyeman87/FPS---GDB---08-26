using System;

[Serializable]
public class WeaponUpgradeData
{
    public string itemID;

    public int damageUpgradeLevel;
    public int shootRateUpgradeLevel;
    public int magSizeUpgradeLevel;

    public WeaponUpgradeData(string id)
    {
        itemID = id;

        damageUpgradeLevel = 0;
        shootRateUpgradeLevel = 0;
        magSizeUpgradeLevel = 0;
    }
}