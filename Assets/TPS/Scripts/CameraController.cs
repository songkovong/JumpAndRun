using Unity.Burst.Intrinsics;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform followTarget;
    [SerializeField] float distance = 3f;
    [SerializeField] float sensitivity = 1f;
    [SerializeField] float verticalSensitivityScale = 0.5f;
    [SerializeField] float minVerticalAngle = -20f;
    [SerializeField] float maxVerticalAngle = 45f;

    Vector2 framingOffset = new Vector2(0, 1);

    [SerializeField] bool invertX;
    [SerializeField] bool invertY;

    float rotationX;
    float rotationY;

    float invertXVal;
    float invertYVal;

    [SerializeField] LayerMask layerMask;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }


    private void Update()
    {
        RaycastHit hit;

        invertXVal = invertX ? -1 : 1;
        invertYVal = invertY ? -1 : 1;

        verticalSensitivityScale = Mathf.Clamp(verticalSensitivityScale, 0.01f, 1f);
        distance = Mathf.Clamp(distance, 1f, 3f);
        rotationX = Mathf.Clamp(rotationX, minVerticalAngle, maxVerticalAngle);

        sensitivity = Mathf.Clamp(sensitivity, 0.01f, 1f);

        rotationX += Input.GetAxis("Mouse Y") * invertXVal * sensitivity * verticalSensitivityScale;
        rotationY += Input.GetAxis("Mouse X") * invertYVal * sensitivity;

        var targetRotation = Quaternion.Euler(rotationX, rotationY, 0);
        var focusPosition = followTarget.position + new Vector3(framingOffset.x, framingOffset.y);

        transform.rotation = targetRotation;

        // Obstacle Camera Movement
        var delta = transform.position - focusPosition;

        //if(Physics.Raycast(focusPosition, delta, out hit, distance, LayerMask.GetMask("Obstacles"))) {
        if(Physics.Raycast(focusPosition, delta, out hit, distance, layerMask)) {
            var _distance = (focusPosition - hit.point).magnitude * 0.8f;
            transform.position = focusPosition - targetRotation * new Vector3(0, 0, _distance);
        } else {
            transform.position = focusPosition - targetRotation * new Vector3(0, 0, distance);
        }
    }

    public Quaternion PlanerRotaion => Quaternion.Euler(0, rotationY, 0);
}
