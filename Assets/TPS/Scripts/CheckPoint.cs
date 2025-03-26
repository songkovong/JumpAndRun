using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * 180 * Time.deltaTime , Space.World);
    }
}
