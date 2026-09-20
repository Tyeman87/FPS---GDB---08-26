using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class MenuLdr
{

    public enum Scene
    {
        Moni, justinScene, testAssault, testProtect, testHostage, LoadingScene

    }

    private static Action onLoaderCallback;
    public static void load(Scene scene)
    {
        onLoaderCallback = () =>
        {
            SceneManager.LoadScene(scene.ToString());
        };

        SceneManager.LoadScene(Scene.LoadingScene.ToString());

    }

    public static void LoaderCallback()
    {
        if(onLoaderCallback != null)
        {
            onLoaderCallback();
            onLoaderCallback = null;
        }
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
