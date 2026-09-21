using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButton : MonoBehaviour
{
    [Header("Scene")]
    [SerializeField] private string gameSceneName;

    public void ContinueGame()
    {
        SaveDataManager.Instance.Load();
        SceneManager.LoadScene(gameSceneName);
    }

    public void NewGame()
    {
        SaveDataManager.Instance.ResetSave();
        SceneManager.LoadScene(gameSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}