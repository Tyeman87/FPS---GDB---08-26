using UnityEngine;
using System.IO;
public static class SaveSystem
{
    private static readonly string SAVE_FOLDER = Path.Combine(Application.persistentDataPath, "Saves");
    private static readonly string SAVE_FILE = Path.Combine(SAVE_FOLDER, "save.json");

    public static void Init()
    {
        //check for save folder
        string folder = Path.GetDirectoryName(SAVE_FOLDER);
        if (!Directory.Exists(folder))
        {
            //create save folder
            Directory.CreateDirectory(folder);
        }
    }

    public static void Save(string saveString)
    {
        File.WriteAllText(SAVE_FOLDER, saveString);

    }

    public static string Load()
    {
        if (File.Exists(SAVE_FOLDER))
        {
            return File.ReadAllText(SAVE_FOLDER);
        }
        return null;

    }
}
