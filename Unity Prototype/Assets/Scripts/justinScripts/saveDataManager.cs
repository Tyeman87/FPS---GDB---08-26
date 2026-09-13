using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;


public class SaveDataManager : MonoBehaviour
{
    //to let shopManager read credits
    public int PlayerCredits => data.playerCredits;


    private class SaveObject
    {
        public int playerCredits;
        public List<string> unlockedItemIDs = new List<string>();
        //public List<string> loadoutIDs = new List<string>();
    }
    public bool IsItemUnlocked(string itemID)
    {
        return data.unlockedItemIDs.Contains(itemID);
    }


    SaveObject data = new SaveObject();

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
        if (Input.GetButtonDown("SaveGame"))
        {
            //increment playerCredits in saveObject for testing
            data.playerCredits += 100;
            Debug.Log($"Total credits: {data.playerCredits}. Saved game.");
            Save();

            ShopManager.Instance.UpdateCreditsUI();
        }

        if (Input.GetButtonDown("LoadGame"))
        {
            Load();
        }

        if (Input.GetButtonDown("ResetSave"))
        {
            ResetSave();
            ShopManager.Instance.UpdateCreditsUI();
        }
    }

    public void Save()
    {
        string json = JsonUtility.ToJson(data);
        SaveSystem.Save(json);
        Debug.Log("Saved player data");
    }

    public void ResetSave()
    {
        data.playerCredits = 0;
        data.unlockedItemIDs.Clear();
        Save();
        Debug.Log($"Save Data has been reset. Player Money: {data.playerCredits}; Unlocked Items: {data.unlockedItemIDs.Count}");
        if (ShopManager.Instance != null)
        {
            ShopManager.Instance.RefreshShopSlots();
        }
    }

    public void Load()
    {
        string saveString = SaveSystem.Load();

        if (saveString != null)
        {
            JsonUtility.FromJsonOverwrite(saveString, data);
            Debug.Log("Loaded: " + saveString);
        }
    }

    public bool Purchase(ItemStats shopItem)
    {
        if (data.playerCredits >= shopItem.itemCost && !data.unlockedItemIDs.Contains(shopItem.itemID))
        {
            //add item to unlockedIDs
            data.unlockedItemIDs.Add(shopItem.itemID);
            //subtract money
            data.playerCredits -= shopItem.itemCost;
            Save();
            return true;
        }
        return false;
    }
}
