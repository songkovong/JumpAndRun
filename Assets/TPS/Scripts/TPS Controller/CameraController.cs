using StarterAssets;
using UnityEditor;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Tooltip("Camera follow target")]
    [SerializeField] Transform followTarget;
    [Tooltip("Distance to camera and follow target")]
    [SerializeField] float distance = 4f;
    [Tooltip("Camera sensitivity")]
    [SerializeField] float sensitivity;
    [Tooltip("Camera vertical sensitivity")]
    [SerializeField] float verticalSensitivityScale;
    [Tooltip("Minimum vertical angle")]
    [SerializeField] float minVerticalAngle = -75f; // 20f
    [Tooltip("Maximum vertical angle")]
    [SerializeField] float maxVerticalAngle = 70f; // 45f
    [Tooltip("Invert X (Vertical camera move)")]
    [SerializeField] bool invertX;
    [Tooltip("Invert Y (Horizontal camera move)")]
    [SerializeField] bool invertY;
    [Tooltip("LayerMask for detective obstacles")]
    [SerializeField] LayerMask layerMask;
    
    private Vector2 framingOffset; // Camera framing offset
    private float rotationX; // Vertical camera move
    private float rotationY; // Horizontal camera move
    private float invertXVal; // If invert vertical ? -1 : 1
    private float invertYVal; // If invert horizontal ? -1 : 1
    private Quaternion currentRotation; // Camera's current rotation
    private Vector3 currentPosition; // Camera's current position
    private float rotationSmoothness = 200.0f; // Smoothness for camera rotation
    private float positionSmoothness = 200.0f; // Smoothness for camera position

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
        distance = Mathf.Clamp(distance, 0.1f, 8f);
        sensitivity = Mathf.Clamp(sensitivity, 0.01f, 2f);

        if(distance <= .5f) {
            GameManager.isTPS = false;
        } else GameManager.isTPS = true;

        GameManager.camDistance = distance;

        rotationX += Input.GetAxis("Mouse Y") * invertXVal * sensitivity * verticalSensitivityScale;
        rotationY += Input.GetAxis("Mouse X") * invertYVal * sensitivity;
        rotationX = Mathf.Clamp(rotationX, minVerticalAngle, maxVerticalAngle);
        distance -= Input.GetAxis("Mouse ScrollWheel") * 2f;

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

        // If obstacle exist to player and camera, adjust camera distance
        if (Physics.Raycast(focusPosition, delta.normalized, out hit, distance, layerMask))
        {
            adjustedDistance = (focusPosition - hit.point).magnitude * 0.8f;
        }
        
        // Set target position and Smoothness
        Vector3 targetPosition = focusPosition - currentRotation * new Vector3(0, 0, adjustedDistance);
        currentPosition = Vector3.Lerp(currentPosition, targetPosition, positionSmoothness * Time.deltaTime);
        transform.position = currentPosition;
    }

    // Camera rotation for player forward vector
    public Quaternion PlanerRotaion => Quaternion.Euler(0, rotationY, 0);
}