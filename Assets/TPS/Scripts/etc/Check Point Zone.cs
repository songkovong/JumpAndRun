using UnityEngine;
using TMPro;

public class CheckPointZone : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private TMP_Text checkPointText;
    [SerializeField] private Vector3 zoneCenter;
    [SerializeField] private Vector3 zoneSize;
    [SerializeField] private float displayTime = 3f;

    private bool _activated = false;

    void Update()
    {
        if (!_activated && IsPlayerInZone())
        {
            _activated = true;
            Debug.Log("체크포인트 진입!");
            StartCoroutine(ShowCheckPointText());
        }
    }

    bool IsPlayerInZone()
    {
        Vector3 playerPos = player.position;
        Vector3 min = zoneCenter - zoneSize * 0.5f;
        Vector3 max = zoneCenter + zoneSize * 0.5f;

        return playerPos.x >= min.x && playerPos.x <= max.x &&
               playerPos.y >= min.y && playerPos.y <= max.y &&
               playerPos.z >= min.z && playerPos.z <= max.z;
    }

    System.Collections.IEnumerator ShowCheckPointText()
    {
        checkPointText.gameObject.SetActive(true);
        yield return new WaitForSeconds(displayTime);
        checkPointText.gameObject.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(zoneCenter, zoneSize);
    }
}
