using UnityEngine;
using UnityEngine.SceneManagement;

public static class MenuLdr
{

    public enum Scene
    {
        Moni, justinScene, testAssault, testProtect, testHostage, shopScene


    }


    public static void load(Scene scene)
    {
        SceneManager.LoadScene(scene.ToString());
        
    }
}
