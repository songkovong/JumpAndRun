using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public void MainScene()
    {
        SceneManager.LoadScene("Main Scene");
    }

    public void LoadingScene()
    {
        GameManager.isPause = false;
        SceneManager.LoadScene("Loading Scene");
    }

    public void GameScene()
    {
        GameManager.isPause = false;
        SceneManager.LoadScene("Game Scene");
    }

    public void GameExit()
    {
        Application.Quit();
    }
}
