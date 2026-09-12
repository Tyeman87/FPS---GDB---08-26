using UnityEngine;
using System.IO;
public static class SaveSystem
{
    private static readonly string SAVE_PATH = Path.Combine(Application.persistentDataPath, "Saves");

    public static void Init()
    {
        //check for save folder
        string folder = Path.GetDirectoryName(SAVE_PATH);
        if (!Directory.Exists(folder))
        {
            //create save folder
            Directory.CreateDirectory(folder);
        }
    }

    public static void Save(string saveString)
    {
        File.WriteAllText(SAVE_PATH, saveString);

    }

    public static string Load()
    {
        if (File.Exists(SAVE_PATH))
        {
            return File.ReadAllText(SAVE_PATH);
        }
        return null;

    }
}
