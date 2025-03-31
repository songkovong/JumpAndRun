using UnityEngine;
using TMPro;
using UnityEngine.Rendering;
using System.Collections;

public class GraphicManager : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown dropdown;
    [SerializeField] private RenderPipelineAsset[] qualityAssets; // 품질별 RenderPipelineAsset 배열

    void Start()
    {
        dropdown.onValueChanged.AddListener(SetQualityLevelDropdown);
        dropdown.value = QualitySettings.GetQualityLevel();
    }

    public void SetQualityLevelDropdown(int index)
    {
        QualitySettings.SetQualityLevel(index, true);
        QualitySettings.renderPipeline = qualityAssets[index]; // 직접 렌더 파이프라인 설정
        Debug.Log($"Quality Level Changed: {index}");
    }
}
