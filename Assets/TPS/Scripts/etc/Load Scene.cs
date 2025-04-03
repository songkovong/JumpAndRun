using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public void MainScene()
    {
        LoadingSceneManager.LoadNextScene("Main Scene");
    }

    public void GameScene()
    {
        GameManager.isPause = false;
        LoadingSceneManager.LoadNextScene("Game Scene");
    }

    public void GameExit()
    {
        Application.Quit();
    }
}
