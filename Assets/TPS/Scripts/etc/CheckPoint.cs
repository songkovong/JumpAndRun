using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    void Update()
    {
        transform.Rotate(Vector3.up * 180 * Time.deltaTime , Space.World);
    }
}
