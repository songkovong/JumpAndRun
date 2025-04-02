using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class SceneTip : MonoBehaviour
{
    public string nextScene = "Game Scene"; // 로드할 씬 이름
    public TMP_Text tipText; // 팁을 표시할 UI Text

    private List<string> tips = new List<string>
    {
        "Tip: 마우스 스크롤을 이용해 카메라 거리를 조절할 수 있습니다.",
        "Tip: 가까운 곳에 장애물이 있다면 스페이스 바를 통해 파쿠르를 사용할 수 있습니다.(미구현)",
        "Tip: 특정 구역에 들어서면 자동으로 체크포인트가 활성화 됩니다.",
        "Tip: 카메라 거리를 가까이 하여 1인칭 시점으로도 플레이 할 수 있습니다.",
        "Tip: 당연하게도 이동은 WASD, 점프는 SPACE, 달리기는 SHIFT 입니다!",
        "Tip: 당신의 컨트롤을 통해 정상에 도달해보세요!",
        "Tip: 모든 길이 정답이니 여러분이 원하는 길로 가보세요!",
    };

    void Start()
    {
        StartCoroutine(ShowLoadingTips());
    }

    IEnumerator ShowLoadingTips()
    {
        ShuffleTips(); // 팁을 랜덤하게 섞음

        for (int i = 0; i < 3; i++) // 3개의 팁을 순차적으로 표시
        {
            tipText.text = tips[i]; // 새로운 팁 표시
            yield return new WaitForSeconds(3f); // 3초 대기
        }

        LoadNextScene(); // 3개 다 표시된 후 씬 전환
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene(nextScene);
    }

    // 팁 리스트를 랜덤하게 섞는 함수
    void ShuffleTips()
    {
        for (int i = 0; i < tips.Count; i++)
        {
            int randomIndex = Random.Range(0, tips.Count);
            string temp = tips[i];
            tips[i] = tips[randomIndex];
            tips[randomIndex] = temp;
        }
    }
}