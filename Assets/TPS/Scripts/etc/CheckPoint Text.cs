using System.Collections;
using TMPro;
using UnityEngine;

public class CheckPointText : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(CheckPointActivateText());
        }
    }

    IEnumerator CheckPointActivateText()
    {
        _text.enabled = true;

        yield return new WaitForSeconds(3f);

        _text.enabled = false;
    }
}
