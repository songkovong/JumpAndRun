using UnityEngine;

public class SmoothPingPongMove : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float speed = 2f;

    void Update()
    {
        float t = (Mathf.Sin(Time.time * speed) + 1) / 2; // 0~1 사이의 값을 반복
        transform.position = Vector3.Lerp(pointA.position, pointB.position, t);
    }
}
