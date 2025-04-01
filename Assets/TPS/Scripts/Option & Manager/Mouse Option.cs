using UnityEngine;
using UnityEngine.UI;

public class MouseOption : MonoBehaviour
{
    // Camera setting
    public Slider sensSlider;
    public Slider verticalSensSlider;


    public void FindSliders()
    {
        // Find Slider
        sensSlider = GameObject.FindWithTag("SensSlider")?.GetComponent<Slider>();
        verticalSensSlider = GameObject.FindWithTag("VerticalSensSlider")?.GetComponent<Slider>();

        if (sensSlider != null)
        {
            sensSlider.value = GameManager.sensitivity;
            sensSlider.onValueChanged.AddListener(SetSensitivity);
        }

        if (verticalSensSlider != null)
        {
            verticalSensSlider.value = GameManager.verticalSensitivityScale;
            verticalSensSlider.onValueChanged.AddListener(SetVerticalSensitivity);
        }

        Debug.Log("Find sens sliders");
    }

    // Set sensitivity
    public void SetSensitivity(float sens)
    {
        GameManager.sensitivity = sens;
        SaveSensSettings();
        Debug.Log("sens = " + GameManager.sensitivity);
    }

    public void SetVerticalSensitivity(float verSens)
    {
        GameManager.verticalSensitivityScale = verSens;
        SaveSensSettings();
        Debug.Log("Versens = " + GameManager.verticalSensitivityScale);
    }

    // Save sensitivity in PlayerPrefs
    public void SaveSensSettings()
    {
        PlayerPrefs.SetFloat("Sens", GameManager.sensitivity);
        PlayerPrefs.SetFloat("VerSens", GameManager.verticalSensitivityScale);
        PlayerPrefs.Save();
        Debug.Log("Sens Save");
    }
}
