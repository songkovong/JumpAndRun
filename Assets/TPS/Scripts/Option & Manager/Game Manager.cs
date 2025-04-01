using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // Declare instance as Static can be accessed from other object
    public static GameManager instance;

    // Pause setting
    public static bool isPause = false; // Pause Panel variable

    // Sound setting
    public static float bgmVolume;
    public static float sfxVolume;

    // Camera setting
    public static float sensitivity;
    public static float verticalSensitivityScale;

    
    void Awake()
    {
        if (instance != null) // If Already exist
        {
            Destroy(gameObject); // Delete one
            return;
        }
        instance = this; // instance self
        DontDestroyOnLoad(gameObject); // Dont destroy

        FrameRateControl();
        LoadVolumeSettings();
        LoadSensSettings();
    }


    public void DeleteSave()
    {
        PlayerPrefs.DeleteKey("Check X");
        PlayerPrefs.DeleteKey("Check Y");
        PlayerPrefs.DeleteKey("Check Z");
    }

    void FrameRateControl()
    {
        Application.targetFrameRate = 120;
    }

    // Load volume in PlayerPrefs
    public void LoadVolumeSettings()
    {
        // PlayerPrefs에서 저장된 볼륨 값이 있는지 확인
        if (PlayerPrefs.HasKey("BGMVolume")) {
            bgmVolume = PlayerPrefs.GetFloat("BGMVolume");
        } else bgmVolume = 0.15f;

        if (PlayerPrefs.HasKey("SFXVolume")) {
            sfxVolume = PlayerPrefs.GetFloat("SFXVolume");
        } else sfxVolume = 0.25f;
    }

    // Load sensitivity in PlayerPrefs
    public void LoadSensSettings()
    {
        // PlayerPrefs에서 저장된 볼륨 값이 있는지 확인
        if (PlayerPrefs.HasKey("Sens")) {
            sensitivity = PlayerPrefs.GetFloat("Sens");
        } else sensitivity = 1f;

        if (PlayerPrefs.HasKey("VerSens")) {
            verticalSensitivityScale = PlayerPrefs.GetFloat("VerSens");
        } else verticalSensitivityScale = 0.5f;
    }
}
