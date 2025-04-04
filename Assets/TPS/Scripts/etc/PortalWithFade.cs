/*using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PortalWithFade : MonoBehaviour
{
    public Transform destination;
    public Image fadeImage;
    public PlayerController playerController;
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
*/


using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PortalWithFade : MonoBehaviour
{
    public Transform destination;
    public Image fadeImage;
    PlayerController playerController;
    Animator playerAnimator;

    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private string portalAnimName = "Look Around";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(TeleportWithFade(other));
        }
    }

    private IEnumerator TeleportWithFade(Collider player)
    {
        if (playerController == null)
            playerController = player.GetComponent<PlayerController>();

        if (playerAnimator == null)
            playerAnimator = player.GetComponent<Animator>();

        playerController.SetControl(false);
        playerAnimator.CrossFade(portalAnimName, 0.1f);

        // Fade In
        for (float i = 0; i <= 1f; i += Time.deltaTime / fadeDuration)
        {
            fadeImage.color = new Color(0, 0, 0, i);
            yield return null;
        }

        // Teleport
        player.transform.position = destination.position - new Vector3(0, 0.5f, 0);


        // Fade Out
        for (float i = 1f; i >= 0f; i -= Time.deltaTime / fadeDuration)
        {
            fadeImage.color = new Color(0, 0, 0, i);
            yield return null;
        }

        playerController.SetControl(true);
    }
}
