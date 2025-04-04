using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    void Update()
    {
        transform.Rotate(Vector3.up * 180f * Time.deltaTime , Space.World);
    }
}
