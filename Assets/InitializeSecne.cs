using UnityEngine;

public class InitializeSecne : MonoBehaviour
{
    [SerializeField] private GameObject optionPanel; // Option Panel

    void Start()
    {
        if (SoundOption.instance != null)
        {
            SoundOption.instance.FindSliders();
            SoundOption.instance.LoadVolumeSettings();
            optionPanel.SetActive(false);
        }
    }
}
