using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevel1 : MonoBehaviour
{
    public void SceneChange()
    {
        SceneManager.LoadScene("Level 1");
    }
}
