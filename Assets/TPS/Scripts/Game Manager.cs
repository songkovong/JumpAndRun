using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // instance를 static으로 선언해서 다른 오브젝트에서도 접근 가능
    public static GameManager instance;

    public static bool isPause = false; // Pause Panel variable

    public static float bgmVolume;
    public static float sfxVolume;
    
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

    void Start()
    {
        FrameRateControl();
        LoadVolumeSettings();
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

    // PlayerPrefs에서 볼륨 값 불러오기
    public void LoadVolumeSettings()
    {
        // PlayerPrefs에서 저장된 볼륨 값이 있는지 확인
        if (PlayerPrefs.HasKey("BGMVolume"))
        {
            bgmVolume = PlayerPrefs.GetFloat("BGMVolume");  // 저장된 BGM 볼륨 불러오기
        } else bgmVolume = 0.15f;

        if (PlayerPrefs.HasKey("SFXVolume"))
        {
            sfxVolume = PlayerPrefs.GetFloat("SFXVolume");  // 저장된 SFX 볼륨 불러오기
        } else sfxVolume = 0.25f;
    }
}
