using UnityEngine;

public class Pause : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel; // Pause Panel

    void Update()
    {
        if(!GameManager.isPause) {
            pausePanel.SetActive(false);
            Time.timeScale = 1f;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void CallMenu()
    {
        Debug.Log("Pause");
        GameManager.isPause = true;
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void CloseMenu()
    {
        Debug.Log("DePause");
        GameManager.isPause = false;
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
