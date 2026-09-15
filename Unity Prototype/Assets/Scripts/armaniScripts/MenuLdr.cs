using UnityEngine;
using UnityEngine.SceneManagement;

public static class MenuLdr
{

    public enum Scene
    {
<<<<<<< Updated upstream
        Moni, justinScene,
=======
        Moni, justinScene, testAssault, testProtect, testHostage, shopScene
>>>>>>> Stashed changes

    }


    public static void load(Scene scene)
    {
        SceneManager.LoadScene(scene.ToString());
        
    }
}
