using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LoadingSceneManager : MonoBehaviour
{
    static string nextScene;
    [SerializeField] Image loadingImage;
    [SerializeField] TMP_Text tipText; // 팁을 표시할 UI Text

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
        StartCoroutine(LoadSceneProgress());
    }
 
    public static void LoadNextScene(string sceneName)
    {
        Debug.Log("LoadNextScene");
        nextScene = sceneName;
        SceneManager.LoadScene("Loading Scene");
    }

    IEnumerator LoadSceneProgress()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false; // Load scene 90%

        tipText.text = ShuffleTips(); // Shuffle Tips

        float timer = 0f;

        while(!op.isDone)
        {
            yield return null;

            if(op.progress < 0.9f) {
                loadingImage.fillAmount = op.progress;
            } else {
                timer += Time.unscaledDeltaTime / 3;
                loadingImage.fillAmount = Mathf.Lerp(0.9f, 1f, timer);
                if(loadingImage.fillAmount >= 1f) {
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
        }
    }

    // 팁 리스트를 랜덤하게 섞는 함수
    string ShuffleTips()
    {
        Debug.Log("ShuffleTips");
        for (int i = 0; i < tips.Count; i++)
        {
            int randomIndex = Random.Range(0, tips.Count);
            string temp = tips[i];
            tips[i] = tips[randomIndex];
            tips[randomIndex] = temp;
        }
        var index = Random.Range(0, tips.Count - 1);
        return tips[index];
    }
}