/*using Unity.Burst.Intrinsics;
using UnityEditor;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform followTarget;
    [SerializeField] float distance = 3f;
    [SerializeField] float sensitivity = 1f;
    [SerializeField] float verticalSensitivityScale = 0.5f;
    [SerializeField] float minVerticalAngle = -20f;
    [SerializeField] float maxVerticalAngle = 45f;

    Vector2 framingOffset;

    [SerializeField] bool invertX;
    [SerializeField] bool invertY;

    float rotationX;
    float rotationY;

    float invertXVal;
    float invertYVal;

    [SerializeField] LayerMask layerMask;

    private void Start()
    {
        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;
        framingOffset = new Vector2(0, 1.3f);
    }


    private void Update()
    {
        if(GameManager.isPause) return; // If pause, Dont rotate camera

        RaycastHit hit;

        invertXVal = invertX ? -1 : 1;
        invertYVal = invertY ? -1 : 1;

        verticalSensitivityScale = Mathf.Clamp(verticalSensitivityScale, 0.01f, 1f);
        distance = Mathf.Clamp(distance, 1f, 5f);
        rotationX = Mathf.Clamp(rotationX, minVerticalAngle, maxVerticalAngle);

        sensitivity = Mathf.Clamp(sensitivity, 0.01f, 1f);

        rotationX += Input.GetAxis("Mouse Y") * invertXVal * sensitivity * verticalSensitivityScale;
        rotationY += Input.GetAxis("Mouse X") * invertYVal * sensitivity;
        distance -= Input.GetAxis("Mouse ScrollWheel");

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
*/

using UnityEditor;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform followTarget;
    [SerializeField] float distance = 3f;
    [SerializeField] float sensitivity;
    [SerializeField] float verticalSensitivityScale;
    [SerializeField] float minVerticalAngle = -20f;
    [SerializeField] float maxVerticalAngle = 45f;
    [SerializeField] bool invertX;
    [SerializeField] bool invertY;
    [SerializeField] LayerMask layerMask;
    
    private Vector2 framingOffset;
    private float rotationX;
    private float rotationY;
    private float invertXVal;
    private float invertYVal;
    private Quaternion currentRotation;
    private Vector3 currentPosition;
    private float rotationSmoothness = 200.0f;
    private float positionSmoothness = 200.0f;
    
    private void Start()
    {
        framingOffset = new Vector2(0, 1.3f);
        currentRotation = transform.rotation;
        currentPosition = transform.position;

        sensitivity = GameManager.sensitivity;
        verticalSensitivityScale = GameManager.verticalSensitivityScale;

        Debug.Log("sens = " + sensitivity);
        Debug.Log("versens = " + verticalSensitivityScale);
    }

    void Update()
    {
        sensitivity = GameManager.sensitivity;
        verticalSensitivityScale = GameManager.verticalSensitivityScale;
    }

    private void LateUpdate()
    {
        if (GameManager.isPause) return; // If pause, don't rotate camera

        invertXVal = invertX ? -1 : 1;
        invertYVal = invertY ? -1 : 1;
        verticalSensitivityScale = Mathf.Clamp(verticalSensitivityScale, 0.01f, 1f);
        distance = Mathf.Clamp(distance, 1f, 5f);
        sensitivity = Mathf.Clamp(sensitivity, 0.01f, 2f);

        rotationX += Input.GetAxis("Mouse Y") * invertXVal * sensitivity * verticalSensitivityScale;
        rotationY += Input.GetAxis("Mouse X") * invertYVal * sensitivity;
        rotationX = Mathf.Clamp(rotationX, minVerticalAngle, maxVerticalAngle);
        distance -= Input.GetAxis("Mouse ScrollWheel");

        // Calculate target rotation and position
        Quaternion targetRotation = Quaternion.Euler(rotationX, rotationY, 0);
        Vector3 focusPosition = followTarget.position + new Vector3(framingOffset.x, framingOffset.y, 0);
        
        // Smooth rotation
        currentRotation = Quaternion.Slerp(currentRotation, targetRotation, rotationSmoothness * Time.deltaTime);
        transform.rotation = currentRotation;

        // Obstacle detection and adjust distance 
        Vector3 delta = transform.position - focusPosition;
        RaycastHit hit;
        float adjustedDistance = distance;

        if (Physics.Raycast(focusPosition, delta.normalized, out hit, distance, layerMask))
        {
            adjustedDistance = (focusPosition - hit.point).magnitude * 0.8f;
        }
        
        // Set target position and Smoothness
        Vector3 targetPosition = focusPosition - currentRotation * new Vector3(0, 0, adjustedDistance);
        currentPosition = Vector3.Lerp(currentPosition, targetPosition, positionSmoothness * Time.deltaTime);
        transform.position = currentPosition;
    }

    public Quaternion PlanerRotaion => Quaternion.Euler(0, rotationY, 0);
}