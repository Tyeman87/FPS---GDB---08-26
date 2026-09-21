using UnityEngine;
using System.Collections.Generic;

public class SaveDataManager : MonoBehaviour
{
    // Lets ShopManager read credits
    public int PlayerCredits => data.playerCredits;

    [System.Serializable]
    public class StashedGunSaveData
    {
        public string itemID;
        public int currMag;
        public int currReserve;
    }

    [System.Serializable]
    private class SaveObject
    {
        public int playerCredits;

        // Items the player has purchased/unlocked
        public List<string> unlockedItemIDs = new List<string>();

        // Guns currently stored in the stash
        public List<StashedGunSaveData> stashGuns = new List<StashedGunSaveData>();

        // Loadouts will be added here later.
    }

    [Header("Item Database")]
    [SerializeField] private List<GunStats> allGuns = new List<GunStats>();

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
                    GameObject go = new GameObject("SaveManager");
                    _instance = go.AddComponent<SaveDataManager>();
                }
            }

            return _instance;
        }
    }

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
        // TEST SAVE
        if (Input.GetButtonDown("SaveGame"))
        {
            

            Debug.Log($"Total credits: {data.playerCredits}. Saved game.");

            Save();

            if (ShopManager.Instance != null)
            {
                ShopManager.Instance.UpdateCreditsUI();
            }
        }

        // TEST LOAD
        if (Input.GetButtonDown("LoadGame"))
        {
            Load();

            if (ShopManager.Instance != null)
            {
                ShopManager.Instance.UpdateCreditsUI();
            }
        }

        // TEST RESET
        if (Input.GetButtonDown("ResetSave"))
        {
            ResetSave();

            if (ShopManager.Instance != null)
            {
                ShopManager.Instance.UpdateCreditsUI();
            }
        }
    }

    public void AddCredits(int amount)
    {
        data.playerCredits += amount;
        Save();

        if (ShopManager.Instance != null)
        {
            ShopManager.Instance.UpdateCreditsUI();
        }
    }

    public bool IsItemUnlocked(string itemID)
    {
        return data.unlockedItemIDs.Contains(itemID);
    }

    public void Save()
    {
        string json = JsonUtility.ToJson(data);

        SaveSystem.Save(json);

        Debug.Log("Saved player data.");
    }

    public void Load()
    {
        string saveString = SaveSystem.Load();

        if (saveString != null)
        {
            JsonUtility.FromJsonOverwrite(saveString, data);

            // Protect against older save files.
            if (data.unlockedItemIDs == null)
            {
                data.unlockedItemIDs = new List<string>();
            }

            if (data.stashGuns == null)
            {
                data.stashGuns = new List<StashedGunSaveData>();
            }

            Debug.Log("Loaded: " + saveString);
        }
    }

    public void ResetSave()
    {
        data.playerCredits = 0;

        data.unlockedItemIDs.Clear();

        data.stashGuns.Clear();

        Save();

        Debug.Log(
            $"Save Data has been reset. " +
            $"Player Money: {data.playerCredits}; " +
            $"Unlocked Items: {data.unlockedItemIDs.Count}; " +
            $"Stash Guns: {data.stashGuns.Count}"
        );

        if (ShopManager.Instance != null)
        {
            ShopManager.Instance.RefreshShopSlots();
        }
    }

    public bool Purchase(ItemStats shopItem)
    {
        if (data.playerCredits >= shopItem.itemCost &&
            !data.unlockedItemIDs.Contains(shopItem.itemID))
        {
            data.unlockedItemIDs.Add(shopItem.itemID);

            data.playerCredits -= shopItem.itemCost;

            Save();

            return true;
        }

        return false;
    }

    // =========================================================
    // STASH
    // =========================================================

    public void SaveStash(List<StashContainer.StashedGun> guns)
    {
        data.stashGuns.Clear();

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

        Save();

        Debug.Log($"Stash saved. Guns: {data.stashGuns.Count}");
    }

    public void LoadStash(StashContainer stash)
    {
        if (stash == null)
        {
            Debug.LogWarning("LoadStash called with a null stash.");
            return;
        }

        stash.ClearStash();

        foreach (StashedGunSaveData saveGun in data.stashGuns)
        {
            if (string.IsNullOrEmpty(saveGun.itemID))
            {
                continue;
            }

            GunStats gun = FindGunByID(saveGun.itemID);

            if (gun == null)
            {
                Debug.LogWarning(
                    $"Could not find GunStats with itemID: {saveGun.itemID}"
                );

                continue;
            }

            stash.StoreGun(
                gun,
                saveGun.currMag,
                saveGun.currReserve,
                false
            );
        }

        Debug.Log($"Stash loaded. Guns: {data.stashGuns.Count}");
    }

    private GunStats FindGunByID(string itemID)
    {
        foreach (GunStats gun in allGuns)
        {
            if (gun != null && gun.itemID == itemID)
            {
                return gun;
            }
        }

        return null;
    }
}