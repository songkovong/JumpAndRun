using UnityEngine;
using TMPro;
using UnityEngine.Rendering;
using System.Collections;

public class GraphicManager : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown dropdown;
    [SerializeField] private RenderPipelineAsset[] qualityAssets; // RenderPipelineAsset Array

    void Start()
    {
        dropdown.onValueChanged.AddListener(SetQualityLevelDropdown);
        if(PlayerPrefs.HasKey("QLevel")) {
            dropdown.value = PlayerPrefs.GetInt("QLevel");
        } else dropdown.value = QualitySettings.GetQualityLevel();
    }

    public void SetQualityLevelDropdown(int index)
    {
        QualitySettings.SetQualityLevel(index, true);
        QualitySettings.renderPipeline = qualityAssets[index]; // Set RenderPipeline
        
        PlayerPrefs.SetInt("QLevel", index);
        PlayerPrefs.Save();

        Debug.Log($"Quality Level Changed: {index}");
    }
}
