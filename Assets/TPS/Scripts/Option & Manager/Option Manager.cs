using UnityEngine;
using UnityEngine.UI;

public class OptionManager : MonoBehaviour
{
    public AudioSource audioSource; 

    // Sound Slider
    public Slider BGMSlider;
    public Slider SFXSlider;
    
    // Mouse Slider
    public Slider sensSlider;
    public Slider verticalSensSlider;

    public float sfxVolume;

    [SerializeField] private GameObject optionPanel; // Pause Panel


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

    public void MainScene()
    {
        GameManager.isPause = false;
        Time.timeScale = 1f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        LoadingSceneManager.LoadNextScene("Main Scene");
    }

    public void GameScene()
    {
        GameManager.isPause = false;
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        LoadingSceneManager.LoadNextScene("Game Scene");
    }

    public void GameExit()
    {
        Application.Quit();
    }

    public void FindSliders()
    {
        // Find Sound Slider
        BGMSlider = GameObject.FindWithTag("BGMSlider")?.GetComponent<Slider>(); 
        SFXSlider = GameObject.FindWithTag("SFXSlider")?.GetComponent<Slider>();

        // Find Mouse Slider
        sensSlider = GameObject.FindWithTag("SensSlider")?.GetComponent<Slider>();
        verticalSensSlider = GameObject.FindWithTag("VerticalSensSlider")?.GetComponent<Slider>();

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

    // Save volume in PlayerPrefs
    public void SaveVolumeSettings()
    {
        PlayerPrefs.SetFloat("BGMVolume", audioSource.volume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.Save();
        Debug.Log("Sound Save");
    }

    // Save sensitivity in PlayerPrefs
    public void SaveSensSettings()
    {
        PlayerPrefs.SetFloat("Sens", GameManager.sensitivity);
        PlayerPrefs.SetFloat("VerSens", GameManager.verticalSensitivityScale);
        PlayerPrefs.Save();
        Debug.Log("Sens Save");
    }
    // Enable Panel
    public void CallMenu()
    {
        Debug.Log("Pause");
        GameManager.isPause = true;
        optionPanel.SetActive(true);
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // Disable Panel
    public void CloseMenu()
    {
        Debug.Log("DePause");
        GameManager.isPause = false;
        optionPanel.SetActive(false);
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
