using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PortalWithFade : MonoBehaviour
{
    public Transform destination;
    public Image fadeImage;
    public MonoBehaviour playerController;
    public Animator playerAnimator;

    [SerializeField] private float fadeDuration = 1.0f;

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

        if (controller != null) controller.enabled = false;
        if (playerController != null) playerController.enabled = false;

        if (playerAnimator != null)
        {
            playerAnimator.Play("Silly Dancing");
        }

        for (float i = 0; i <= 1; i += Time.deltaTime / fadeDuration)
        {
            fadeImage.color = new Color(0, 0, 0, i);
            yield return null;
        }

        player.transform.position = destination.position;

        for (float i = 1; i >= 0; i -= Time.deltaTime / fadeDuration)
        {
            fadeImage.color = new Color(0, 0, 0, i);
            yield return null;
        }

        if (controller != null) controller.enabled = true;
        if (playerController != null) playerController.enabled = true;
    }
}
