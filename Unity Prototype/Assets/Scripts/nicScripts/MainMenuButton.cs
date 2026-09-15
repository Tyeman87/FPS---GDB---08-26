using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButton : MonoBehaviour
{
    [Header("Scene")]
    [SerializeField] private string gameSceneName;

    public void ContinueGame()
    {
        SaveDataManager.Instance.Load();

        Debug.Log("Continuing game with existing save data.");

        SceneManager.LoadScene(gameSceneName);
    }

    public void NewGame()
    {
        SaveDataManager.Instance.ResetSave();

        Debug.Log("Starting New Game. Save data has been reset.");

        SceneManager.LoadScene(gameSceneName);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game.");

        Application.Quit();
    }
}
