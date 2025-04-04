using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundOption : MonoBehaviour
{
    public AudioSource audioSource; 

    // Slider
    public Slider BGMSlider;
    public Slider SFXSlider;

    public float sfxVolume;


    private void Awake()
    {
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>(); // AudioSource Auto add
        }

        audioSource.loop = true; // Set BGM loop play
    }

    void Start()
    {
        audioSource.volume = GameManager.bgmVolume;
        sfxVolume = GameManager.sfxVolume;
    }

    public void FindSliders()
    {
        // Find Slider
        BGMSlider = GameObject.FindWithTag("BGMSlider")?.GetComponent<Slider>(); 
        SFXSlider = GameObject.FindWithTag("SFXSlider")?.GetComponent<Slider>();

        if (BGMSlider != null)
        {
            Debug.Log(BGMSlider);
            audioSource.volume = GameManager.bgmVolume;
            BGMSlider.value = audioSource.volume;
            BGMSlider.onValueChanged.AddListener(SetBGMVolume);
        }

        if (SFXSlider != null)
        {
            Debug.Log(SFXSlider);
            sfxVolume = GameManager.sfxVolume;
            SFXSlider.value = sfxVolume;
            SFXSlider.onValueChanged.AddListener(SetSFXVolume);
        }
    }

    // Set volume
    public void SetBGMVolume(float volume)
    {
        GameManager.bgmVolume = volume;
        audioSource.volume = volume;
        SaveVolumeSettings();
    }

    public void SetSFXVolume(float volume)
    {
        GameManager.sfxVolume = volume;
        sfxVolume = volume;
        SaveVolumeSettings();
    }

    // Save volume in PlayerPrefs
    public void SaveVolumeSettings()
    {
        PlayerPrefs.SetFloat("BGMVolume", audioSource.volume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.Save();
        Debug.Log("Sound Save");
    }

    // Load volume in PlayerPrefs
    /*public void LoadVolumeSettings()
    {
        // PlayerPrefs에서 저장된 볼륨 값이 있는지 확인
        if (PlayerPrefs.HasKey("BGMVolume"))
        {
            audioSource.volume = PlayerPrefs.GetFloat("BGMVolume");  // 저장된 BGM 볼륨 불러오기
        }

        if (PlayerPrefs.HasKey("SFXVolume"))
        {
            sfxVolume = PlayerPrefs.GetFloat("SFXVolume");  // 저장된 SFX 볼륨 불러오기
        }
    }*/
}
