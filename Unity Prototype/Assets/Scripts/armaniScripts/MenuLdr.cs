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

    public static void loadAdditive(Scene scene)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(scene.ToString(), LoadSceneMode.Additive);
        op.completed += _ =>
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(scene.ToString()));
        };
    }

}
