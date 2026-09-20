using System.Collections.Generic;
using UnityEngine;

public class StashContainer : MonoBehaviour, IInteractable
{
    [SerializeField] private StashUI stashUI;
    [SerializeField] private GameObject stashPrompt;
    [SerializeField] private playerController player;

    [System.Serializable]
    public class StashedGun
    {
        public GunStats stats;
        public int currMag;
        public int currReserve;

        public StashedGun(GunStats gun)
        {
            stats = gun;
            currMag = gun.magSize;
            currReserve = gun.maxReserve;
        }

        public StashedGun(GunStats gun, int mag, int reserve)
        {
            stats = gun;
            currMag = mag;
            currReserve = reserve;
        }
    }

    [Header("Loadout Guns")]
    [SerializeField] private List<StashedGun> storedGuns = new List<StashedGun>();

    private bool waitingForInput = false;

    private void Start()
    {
        RefreshFromLoadout();
    }

    public void Interact()
    {
        Debug.Log("Stash Interact called!");

        RefreshFromLoadout();

        stashPrompt.SetActive(true);
        waitingForInput = true;
    }

    public void RefreshFromLoadout()
    {
        storedGuns.Clear();

        if (SaveDataManager.Instance == null)
        {
            Debug.LogWarning("StashContainer: SaveDataManager not found.");
            return;
        }

        int presetIndex = SaveDataManager.Instance.GetActiveLoadoutPresetIndex();
        SaveDataManager.LoadoutPresetSaveData preset = SaveDataManager.Instance.GetLoadoutPreset(presetIndex);

        if (preset == null)
        {
            Debug.LogWarning("StashContainer: Active loadout preset was not found.");
            return;
        }

        AddLoadoutGun(preset.gun1ID);
        AddLoadoutGun(preset.gun2ID);

        Debug.Log("Stash refreshed from Loadout Preset " + (presetIndex + 1) + ". Guns available: " + storedGuns.Count);
    }

    private void AddLoadoutGun(string itemID)
    {
        if (string.IsNullOrEmpty(itemID))
        {
            return;
        }

        GunStats gun = SaveDataManager.Instance.GetGunByID(itemID);

        if (gun == null)
        {
            Debug.LogWarning("StashContainer: Could not find loadout gun with ID " + itemID);
            return;
        }

        foreach (StashedGun existingGun in storedGuns)
        {
            if (existingGun.stats == gun)
            {
                return;
            }
        }

        int magSize = GetCurrentMagSize(gun);
        StashedGun loadoutGun = new StashedGun(gun, magSize, gun.maxReserve);

        storedGuns.Add(loadoutGun);
    }

    private int GetCurrentMagSize(GunStats gun)
    {
        if (gun == null)
        {
            return 0;
        }

        if (SaveDataManager.Instance == null)
        {
            return gun.magSize;
        }

        WeaponUpgradeData upgradeData = SaveDataManager.Instance.GetWeaponUpgradeData(gun.itemID);

        if (upgradeData == null)
        {
            return gun.magSize;
        }

        int magSize = gun.magSize + (gun.magSizeUpgradeAmount * upgradeData.magSizeUpgradeLevel);
        int maxMagSize = gun.maxMagSize > 0 ? gun.maxMagSize : gun.magSize;

        return Mathf.Min(magSize, maxMagSize);
    }

    public List<StashedGun> GetStoredGuns()
    {
        return storedGuns;
    }

    public void RemoveGun(StashedGun gun)
    {
        if (gun == null)
        {
            return;
        }

        storedGuns.Remove(gun);
    }

    public void StoreGun(GunStats gun, int currMag, int currReserve)
    {
        if (gun == null)
        {
            return;
        }

        StashedGun newGun = new StashedGun(gun, currMag, currReserve);
        storedGuns.Add(newGun);
    }

    public void ClearStash()
    {
        storedGuns.Clear();
    }

    private void Update()
    {
        if (!stashPrompt.activeSelf)
        {
            return;
        }

        if (Input.GetKeyUp(KeyCode.E))
        {
            waitingForInput = false;
        }

        if (waitingForInput)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            RefreshFromLoadout();

            stashUI.OpenStash(this);
            stashPrompt.SetActive(false);
        }
    }
}