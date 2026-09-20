using UnityEngine;
using System.Collections.Generic;
public class SaveDataManager : MonoBehaviour
{
    public int PlayerCredits => data.playerCredits;
    [System.Serializable] public class StashedGunSaveData
    {
        public string itemID;
        public int currMag;
        public int currReserve;
    }

    [System.Serializable] public class LoadoutPresetSaveData
    {
        public string gun1ID = "";
        public string gun2ID = "";
        public string grenade1ID = "";
        public string grenade2ID = "";
        public bool initialized = false;
    }

    [System.Serializable] private class SaveObject
    {
        public int playerCredits;
        public List<string> unlockedItemIDs = new List<string>();
        public List<StashedGunSaveData> stashGuns = new List<StashedGunSaveData>();
        public List<StashedGunSaveData> playerGuns = new List<StashedGunSaveData>();
        public List<WeaponUpgradeData> weaponUpgrades = new List<WeaponUpgradeData>();
        public List<LoadoutPresetSaveData> loadoutPresets = new List<LoadoutPresetSaveData>();
        public int activeLoadoutPresetIndex = 0;
        public int equippedGunIndex = 0;
        public bool startingGearGiven = false;
    }

    [Header("Gun Database")] [SerializeField] private List<GunStats> allGuns = new List<GunStats>();
    [Header("Grenade Database")] [SerializeField] private List<GrenadeItemStats> allGrenades = new List<GrenadeItemStats>();
    [Header("Default Unlocked Guns")] [SerializeField] private List<GunStats> defaultUnlockedGuns = new List<GunStats>();
    [Header("Default Unlocked Grenades")] [SerializeField] private List<GrenadeItemStats> defaultUnlockedGrenades = new List<GrenadeItemStats>();
    private SaveObject data = new SaveObject();
    private static SaveDataManager _instance;
    public static SaveDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<SaveDataManager>();
                if (_instance == null)
                {
                    Debug.LogError("SaveDataManager is missing from the scene.");
                }
            }

            return _instance;
        }
    }

    [Header("Auto Save")] [SerializeField] private float autoSaveInterval = 300f;
    private float autoSaveTimer;
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
        SaveSystem.Init();
        Load();
    }

    private void Update()
    {
        autoSaveTimer += Time.deltaTime;
        if (autoSaveTimer >= autoSaveInterval)
        {
            SaveAll();
            autoSaveTimer = 0f;
            Debug.Log("Autosaved game.");
        }

        if (Input.GetButtonDown("SaveGame"))
        {
            data.playerCredits += 100;
            Save();
            if (ShopManager.Instance != null)
            {
                ShopManager.Instance.UpdateCreditsUI();
            }
        }

        if (Input.GetButtonDown("LoadGame"))
        {
            Load();
            if (ShopManager.Instance != null)
            {
                ShopManager.Instance.UpdateCreditsUI();
                ShopManager.Instance.RefreshShopSlots();
            }
        }

        if (Input.GetButtonDown("ResetSave"))
        {
            ResetSave();
        }
    }

    private void EnsureSaveCollections()
    {
        if (data.unlockedItemIDs == null)
        {
            data.unlockedItemIDs = new List<string>();
        }

        if (data.stashGuns == null)
        {
            data.stashGuns = new List<StashedGunSaveData>();
        }

        if (data.playerGuns == null)
        {
            data.playerGuns = new List<StashedGunSaveData>();
        }

        if (data.weaponUpgrades == null)
        {
            data.weaponUpgrades = new List<WeaponUpgradeData>();
        }

        if (data.loadoutPresets == null)
        {
            data.loadoutPresets = new List<LoadoutPresetSaveData>();
        }

        EnsureLoadoutPresets();
    }

    private void EnsureLoadoutPresets()
    {
        while (data.loadoutPresets.Count < 3)
        {
            data.loadoutPresets.Add(new LoadoutPresetSaveData());
        }

        foreach (LoadoutPresetSaveData preset in data.loadoutPresets)
        {
            if (preset == null)
            {
                continue;
            }

            bool containsSavedItems = !string.IsNullOrEmpty(preset.gun1ID) || !string.IsNullOrEmpty(preset.gun2ID) || !string.IsNullOrEmpty(preset.grenade1ID) || !string.IsNullOrEmpty(preset.grenade2ID);
            if (containsSavedItems)
            {
                preset.initialized = true;
            }
        }

        data.activeLoadoutPresetIndex = Mathf.Clamp(data.activeLoadoutPresetIndex, 0, 2);
    }

    private bool AddDefaultUnlockedItems()
    {
        EnsureSaveCollections();
        bool changed = false;
        foreach (GunStats gun in defaultUnlockedGuns)
        {
            if (gun == null)
            {
                continue;
            }

            if (string.IsNullOrEmpty(gun.itemID))
            {
                Debug.LogWarning(gun.name + " has no Item ID.");
                continue;
            }

            if (!data.unlockedItemIDs.Contains(gun.itemID))
            {
                data.unlockedItemIDs.Add(gun.itemID);
                changed = true;
                Debug.Log("DEFAULT GUN UNLOCKED: " + gun.itemName);
            }
        }

        foreach (GrenadeItemStats grenade in defaultUnlockedGrenades)
        {
            if (grenade == null)
            {
                continue;
            }

            if (string.IsNullOrEmpty(grenade.itemID))
            {
                Debug.LogWarning(grenade.name + " has no Item ID.");
                continue;
            }

            if (!data.unlockedItemIDs.Contains(grenade.itemID))
            {
                data.unlockedItemIDs.Add(grenade.itemID);
                changed = true;
                Debug.Log("DEFAULT GRENADE UNLOCKED: " + grenade.itemName);
            }
        }

        return changed;
    }

    public bool IsItemUnlocked(string itemID)
    {
        if (string.IsNullOrEmpty(itemID))
        {
            return false;
        }

        EnsureSaveCollections();
        return data.unlockedItemIDs.Contains(itemID);
    }

    public void Save()
    {
        EnsureSaveCollections();
        string json = JsonUtility.ToJson(data);
        SaveSystem.Save(json);
        Debug.Log("Saved player data.");
    }

    public void SaveAll()
    {
        playerController player = FindAnyObjectByType<playerController>();
        StashContainer stash = FindAnyObjectByType<StashContainer>();
        if (player != null)
        {
            SavePlayerInventory(player.GetGunInventory(), player.GetGunIndex());
        }

        if (stash != null)
        {
            SaveStash(stash.GetStoredGuns());
        }

        Save();
    }

    public void Load()
    {
        string saveString = SaveSystem.Load();
        if (!string.IsNullOrEmpty(saveString))
        {
            JsonUtility.FromJsonOverwrite(saveString, data);
            EnsureSaveCollections();
            bool changed = AddDefaultUnlockedItems();
            if (changed)
            {
                Save();
            }

            Debug.Log("Save loaded.");
            return;
        }

        data = new SaveObject();
        EnsureSaveCollections();
        AddDefaultUnlockedItems();
        Save();
        Debug.Log("New save created.");
    }

    public void ResetSave()
    {
        data = new SaveObject();
        EnsureSaveCollections();
        AddDefaultUnlockedItems();
        Save();
        if (ShopManager.Instance != null)
        {
            ShopManager.Instance.UpdateCreditsUI();
            ShopManager.Instance.RefreshShopSlots();
        }

        Debug.Log("Save reset.");
    }

    public bool Purchase(ItemStats shopItem)
    {
        if (shopItem == null)
        {
            Debug.LogWarning("Purchase failed: item is null.");
            return false;
        }

        EnsureSaveCollections();
        if (IsItemUnlocked(shopItem.itemID))
        {
            Debug.Log(shopItem.itemName + " is already owned.");
            return false;
        }

        if (data.playerCredits < shopItem.itemCost)
        {
            Debug.Log("Not enough credits for " + shopItem.itemName);
            return false;
        }

        data.playerCredits -= shopItem.itemCost;
        data.unlockedItemIDs.Add(shopItem.itemID);
        GunStats purchasedGun = shopItem as GunStats;
        if (purchasedGun != null)
        {
            AddGunToStash(purchasedGun);
        }

        Save();
        if (ShopManager.Instance != null)
        {
            ShopManager.Instance.UpdateCreditsUI();
            ShopManager.Instance.RefreshShopSlots();
        }

        Debug.Log("Purchased " + shopItem.itemName);
        return true;
    }

    private void AddGunToStash(GunStats gun)
    {
        if (gun == null)
        {
            return;
        }

        EnsureSaveCollections();
        foreach (StashedGunSaveData savedGun in data.stashGuns)
        {
            if (savedGun.itemID == gun.itemID)
            {
                return;
            }
        }

        StashedGunSaveData newGun = new StashedGunSaveData();
        newGun.itemID = gun.itemID;
        newGun.currMag = GetCurrentMagSizeForGun(gun);
        newGun.currReserve = gun.maxReserve;
        data.stashGuns.Add(newGun);
    }

    private int GetCurrentMagSizeForGun(GunStats gun)
    {
        if (gun == null)
        {
            return 0;
        }

        WeaponUpgradeData upgradeData = GetWeaponUpgradeData(gun.itemID);
        int magSize = gun.magSize + (gun.magSizeUpgradeAmount * upgradeData.magSizeUpgradeLevel);
        int maxMagSize = gun.maxMagSize > 0 ? gun.maxMagSize : gun.magSize;
        return Mathf.Min(magSize, maxMagSize);
    }

    public WeaponUpgradeData GetWeaponUpgradeData(string itemID)
    {
        if (string.IsNullOrEmpty(itemID))
        {
            return null;
        }

        EnsureSaveCollections();
        foreach (WeaponUpgradeData upgradeData in data.weaponUpgrades)
        {
            if (upgradeData.itemID == itemID)
            {
                return upgradeData;
            }
        }

        WeaponUpgradeData newUpgradeData = new WeaponUpgradeData(itemID);
        data.weaponUpgrades.Add(newUpgradeData);
        return newUpgradeData;
    }

    public void SaveWeaponUpgrades()
    {
        Save();
        Debug.Log("Weapon upgrades saved.");
    }

    public void SaveStash(List<StashContainer.StashedGun> guns)
    {
        EnsureSaveCollections();
        data.stashGuns.Clear();
        if (guns == null)
        {
            return;
        }

        foreach (StashContainer.StashedGun gun in guns)
        {
            if (gun == null || gun.stats == null)
            {
                continue;
            }

            StashedGunSaveData saveGun = new StashedGunSaveData();
            saveGun.itemID = gun.stats.itemID;
            saveGun.currMag = gun.currMag;
            saveGun.currReserve = gun.currReserve;
            data.stashGuns.Add(saveGun);
        }
    }

    public void LoadStash(StashContainer stash)
    {
        if (stash == null)
        {
            return;
        }

        EnsureSaveCollections();
        stash.ClearStash();
        foreach (StashedGunSaveData saveGun in data.stashGuns)
        {
            GunStats gun = FindGunByID(saveGun.itemID);
            if (gun == null)
            {
                continue;
            }

            stash.StoreGun(gun, saveGun.currMag, saveGun.currReserve);
        }
    }

    private GunStats FindGunByID(string itemID)
    {
        if (string.IsNullOrEmpty(itemID))
        {
            return null;
        }

        foreach (GunStats gun in allGuns)
        {
            if (gun != null && gun.itemID == itemID)
            {
                return gun;
            }
        }

        return null;
    }

    public GunStats GetGunByID(string itemID)
    {
        return FindGunByID(itemID);
    }

    public List<GunStats> GetUnlockedGuns()
    {
        EnsureSaveCollections();
        List<GunStats> result = new List<GunStats>();
        foreach (GunStats gun in allGuns)
        {
            if (gun == null)
            {
                continue;
            }

            if (IsItemUnlocked(gun.itemID))
            {
                result.Add(gun);
            }
        }

        return result;
    }

    private GrenadeItemStats FindGrenadeByID(string itemID)
    {
        if (string.IsNullOrEmpty(itemID))
        {
            return null;
        }

        foreach (GrenadeItemStats grenade in allGrenades)
        {
            if (grenade != null && grenade.itemID == itemID)
            {
                return grenade;
            }
        }

        return null;
    }

    public GrenadeItemStats GetGrenadeByID(string itemID)
    {
        return FindGrenadeByID(itemID);
    }

    public List<GrenadeItemStats> GetUnlockedGrenades()
    {
        EnsureSaveCollections();
        List<GrenadeItemStats> result = new List<GrenadeItemStats>();
        foreach (GrenadeItemStats grenade in allGrenades)
        {
            if (grenade == null)
            {
                continue;
            }

            if (IsItemUnlocked(grenade.itemID))
            {
                result.Add(grenade);
            }
        }

        return result;
    }

    public LoadoutPresetSaveData GetLoadoutPreset(int presetIndex)
    {
        EnsureSaveCollections();
        presetIndex = Mathf.Clamp(presetIndex, 0, 2);
        return data.loadoutPresets[presetIndex];
    }

    public void SaveLoadoutPreset(int presetIndex, GunStats gun1, GunStats gun2, GrenadeItemStats grenade1, GrenadeItemStats grenade2)
    {
        EnsureSaveCollections();
        presetIndex = Mathf.Clamp(presetIndex, 0, 2);
        LoadoutPresetSaveData preset = data.loadoutPresets[presetIndex];
        preset.gun1ID = gun1 != null ? gun1.itemID : "";
        preset.gun2ID = gun2 != null ? gun2.itemID : "";
        preset.grenade1ID = grenade1 != null ? grenade1.itemID : "";
        preset.grenade2ID = grenade2 != null ? grenade2.itemID : "";
        preset.initialized = true;
        data.activeLoadoutPresetIndex = presetIndex;
        Save();
        Debug.Log("Saved Preset " + (presetIndex + 1) + " | Guns: " + (gun1 != null ? gun1.itemName : "None") + ", " + (gun2 != null ? gun2.itemName : "None") + " | Grenades: " + (grenade1 != null ? grenade1.itemName : "None") + ", " + (grenade2 != null ? grenade2.itemName : "None"));
    }

    public void SaveLoadoutPreset(int presetIndex, GunStats gun1, GunStats gun2)
    {
        LoadoutPresetSaveData existing = GetLoadoutPreset(presetIndex);
        GrenadeItemStats grenade1 = GetGrenadeByID(existing.grenade1ID);
        GrenadeItemStats grenade2 = GetGrenadeByID(existing.grenade2ID);
        SaveLoadoutPreset(presetIndex, gun1, gun2, grenade1, grenade2);
    }

    public int GetActiveLoadoutPresetIndex()
    {
        EnsureSaveCollections();
        return Mathf.Clamp(data.activeLoadoutPresetIndex, 0, 2);
    }

    public void SetActiveLoadoutPresetIndex(int presetIndex)
    {
        EnsureSaveCollections();
        data.activeLoadoutPresetIndex = Mathf.Clamp(presetIndex, 0, 2);
        Save();
    }

    public bool StartingGearGiven()
    {
        return data.startingGearGiven;
    }

    public void MarkStartingGearGiven()
    {
        data.startingGearGiven = true;
        Save();
    }

    public void SavePlayerInventory(List<playerController.GunAmmoData> guns, int equippedIndex)
    {
        EnsureSaveCollections();
        data.playerGuns.Clear();
        if (guns != null)
        {
            foreach (playerController.GunAmmoData gun in guns)
            {
                if (gun == null || gun.stats == null)
                {
                    continue;
                }

                StashedGunSaveData saveGun = new StashedGunSaveData();
                saveGun.itemID = gun.stats.itemID;
                saveGun.currMag = gun.currMag;
                saveGun.currReserve = gun.currReserve;
                data.playerGuns.Add(saveGun);
            }
        }

        data.equippedGunIndex = equippedIndex;
    }

    public void LoadPlayerInventory(playerController player)
    {
        if (player == null)
        {
            return;
        }

        EnsureSaveCollections();
        player.ClearGunInventory();
        foreach (StashedGunSaveData saveGun in data.playerGuns)
        {
            GunStats gun = FindGunByID(saveGun.itemID);
            if (gun == null)
            {
                continue;
            }

            player.AddStoredGun(gun, saveGun.currMag, saveGun.currReserve);
        }

        if (player.GetGunInventory().Count > 0)
        {
            int safeIndex = Mathf.Clamp(data.equippedGunIndex, 0, player.GetGunInventory().Count - 1);
            player.SetGunIndex(safeIndex);
        }
    }
}
