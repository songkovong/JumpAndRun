using Unity.Burst.Intrinsics;
using UnityEditor.Search;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform followTarget;
    [SerializeField] float distance = 3f;
    [SerializeField] float sensitivity = 200f;
    [SerializeField] float verticalSensitivityScale = 0.5f;
    [SerializeField] float minVerticalAngle = -20f;
    [SerializeField] float maxVerticalAngle = 45f;

    [SerializeField] Vector2 framingOffset;

    [SerializeField] bool invertX;
    [SerializeField] bool invertY;

    float rotationX;
    float rotationY;

    float invertXVal;
    float invertYVal;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }


    private void Update()
    {
        invertXVal = invertX ? -1 : 1;
        invertYVal = invertY ? -1 : 1;

        verticalSensitivityScale = Mathf.Clamp(verticalSensitivityScale, 0.01f, 1f);
        distance = Mathf.Clamp(distance, 1f, 5f);
        rotationX = Mathf.Clamp(rotationX, minVerticalAngle, maxVerticalAngle);

        rotationX += Input.GetAxis("Mouse Y") * invertXVal * sensitivity * verticalSensitivityScale * Time.deltaTime;
        rotationY += Input.GetAxis("Mouse X") * invertYVal * sensitivity * Time.deltaTime;

        var targetRotation = Quaternion.Euler(rotationX, rotationY, 0);
        var focusPosition = followTarget.position + new Vector3(framingOffset.x, framingOffset.y);

        transform.position = focusPosition - targetRotation * new Vector3(0, 0, distance); 
        transform.rotation = targetRotation;
    }

    public Quaternion PlanerRotaion => Quaternion.Euler(0, rotationY, 0);
}
