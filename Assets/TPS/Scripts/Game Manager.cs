using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // instance를 static으로 선언해서 다른 오브젝트에서도 접근 가능
    public static GameManager instance;

    public static bool isPause = false; // Pause Panel variable
    
    void Awake()
    {
        if (instance != null) // If Already exist
        {
            Destroy(gameObject); // Delete one
            return;
        }
        instance = this; // instance self
        DontDestroyOnLoad(gameObject); // Dont destroy
    }

    public void DeleteSave()
    {
        PlayerPrefs.DeleteAll();
    }
}
