using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;


public class SaveDataManager : MonoBehaviour
{
    

    private class SaveObject
    {
        public int playerCredits;
        public List<string> unlockedItemIDs = new List<string>();
        //public List<string> loadoutIDs = new List<string>();
    }

    SaveObject data = new SaveObject();

    private void Awake()
    {
        SaveSystem.Init();
        
        if (data.unlockedItemIDs.Count == 0)
        {
            data.unlockedItemIDs.Add("test_item_01");
            Save();
        }

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            //increment playerCredits in saveObject
            data.playerCredits += 100;
            Debug.Log(data.playerCredits);
            //call save function
            Save();
            //log new credit amount to console and HUD


        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            Load();
        }
    }

    public void Save()
    {
        string json = JsonUtility.ToJson(data);

        SaveSystem.Save(json);

        Debug.Log("Saved player data");
        //write player data to json
        //pass data variable into json util
    }

    public void Load()
    {
        string saveString = SaveSystem.Load();
        if (saveString != null)
        {
            Debug.Log("Loaded: " + saveString);

            SaveObject saveObject = JsonUtility.FromJson<SaveObject>(saveString);

            data = JsonUtility.FromJson<SaveObject>(saveString);

        }
    }
}
