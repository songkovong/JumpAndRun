using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PortalWithFade : MonoBehaviour
{
    public Transform destination;
    public Image fadeImage; // 검은색 UI 이미지 (Inspector에서 연결)
    public MonoBehaviour playerController; // 플레이어의 컨트롤 스크립트 (Inspector에서 할당)
    public Animator playerAnimator; // 플레이어의 애니메이터 (Inspector에서 연결)

    private float fadeDuration = 2.0f; // 페이드 인/아웃 시간

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(TeleportWithFade(other));
        }
    }

    private IEnumerator TeleportWithFade(Collider player)
    {
        CharacterController controller = player.GetComponent<CharacterController>();

        // 🔹 1. 이동 & 입력 차단
        if (controller != null) controller.enabled = false;
        if (playerController != null) playerController.enabled = false;

        // 🔹 2. Idle 애니메이션 실행 (Idle 상태로 변경)
        if (playerAnimator != null)
        {
            playerAnimator.SetFloat("moveAmount", 0f);
        }

        // 🔹 3. 페이드 인 (화면 어두워짐)
        for (float i = 0; i <= 1; i += Time.deltaTime / fadeDuration)
        {
            fadeImage.color = new Color(0, 0, 0, i);
            yield return null;
        }

        // 🔹 4. 위치 이동
        player.transform.position = destination.position;

        // 🔹 5. 페이드 아웃 (화면 밝아짐)
        for (float i = 1; i >= 0; i -= Time.deltaTime / fadeDuration)
        {
            fadeImage.color = new Color(0, 0, 0, i);
            yield return null;
        }

        // 🔹 6. 이동 & 입력 다시 활성화
        if (controller != null) controller.enabled = true;
        if (playerController != null) playerController.enabled = true;
    }
}
