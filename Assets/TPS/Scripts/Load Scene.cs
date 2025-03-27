using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public void MainScene()
    {
        SceneManager.LoadScene("Main Scene");
    }

    public void GameScene()
    {
        GameManager.isPause = false;
        SceneManager.LoadScene("Game Scene");
    }

    public void DeleteSave()
    {
        PlayerPrefs.DeleteAll();
    }
}
