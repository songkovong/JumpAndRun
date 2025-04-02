using UnityEngine;

public class DeleteManager : MonoBehaviour
{
    public void DeleteSave()
    {
        PlayerPrefs.DeleteKey("Check X");
        PlayerPrefs.DeleteKey("Check Y");
        PlayerPrefs.DeleteKey("Check Z");
    }
}
