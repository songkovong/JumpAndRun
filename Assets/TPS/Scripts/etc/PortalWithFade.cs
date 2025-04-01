using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PortalWithFade : MonoBehaviour
{
    public Transform destination;
    public Image fadeImage;
    public MonoBehaviour playerController;
    public Animator playerAnimator;

    [SerializeField] private float fadeDuration = 1f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(TeleportWithFade(other));
        }
    }

    private IEnumerator TeleportWithFade(Collider player)
    {

        if (playerController != null) playerController.enabled = false;

        if (playerAnimator != null)
        {
            playerAnimator.CrossFade("Look Around", 0.1f);
        }

        for (float i = 0; i <= 1; i += Time.deltaTime / fadeDuration)
        {
            fadeImage.color = new Color(0, 0, 0, i);
            yield return null;
        }

        player.transform.position = destination.position - new Vector3(0, 1, 0);

        for (float i = 1; i >= 0; i -= Time.deltaTime / fadeDuration)
        {
            fadeImage.color = new Color(0, 0, 0, i);
            yield return null;
        }

        if (playerController != null) playerController.enabled = true;
    }
}
