using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public AudioClip LandingAudioClip;
    public AudioClip[] FootstepAudioClips;
    [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

    [Header("Speed Settings")]
    [SerializeField] float moveSpeed = 1.5f;
    [SerializeField] float runSpeed = 5f;
    [SerializeField] float finalSpeed;

    [Header("Rotation Settings")]
    [SerializeField] float rotationSpeed = 500f;
    [SerializeField] float runRotationSpeed = 500f;
    [SerializeField] float finalRotationSpeed;

    [Header("Ground Check Settings")]
    [SerializeField] float groundCheckRadius = 0.2f; // default = 0.2f
    [SerializeField] Vector3 groundCheckOffset = new Vector3(0, 0.1f, 0.04f); // default = (0, 0.15, 0.08)
    [SerializeField] LayerMask groundLayer; // default = Obstacles

    bool isGrounded;

    float ySpeed;
    [SerializeField] float groundGravity = -5f; // -0.5f

    [Header("Jump Settings")]
    [SerializeField] float jumpHeight = 1f;

    [Header("Jump Timeout")]
    [SerializeField] float jumpTimeout = 0.5f;
    float jumpTimeoutDelta;

    [Header("Fall Timeout")]
    [SerializeField] float fallTimeout = 0.15f;
    float fallTimeoutDelta;

    Vector3 moveInput;
    Vector3 moveDir;
    Vector3 velocity;

    Quaternion targetRotation;

    CameraController cameraController;
    Animator animator;
    CharacterController characterController;


    [Header("Pause")]
    [SerializeField] Pause pause;

    [Header("Check Point")]
    [SerializeField] float checkX;
    [SerializeField] float checkY;
    [SerializeField] float checkZ;

    bool isRun = false;

    void Awake()
    {
        cameraController = Camera.main.GetComponent<CameraController>();
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
    }

    void Start()
    {
        jumpTimeoutDelta = jumpTimeout;
        fallTimeoutDelta = fallTimeout;

        LoadCheckPoint();
        isGrounded = true;
    }

    private void Update()
    {
        // check
        CheckPoint();

        // Is Player Run?
        IsRun();

        // Is Grounded?
        GroundCheck();

        // Pause
        PauseGame();

        // Gravity
        if(isGrounded) {
            ySpeed = groundGravity;

            fallTimeoutDelta = fallTimeout;

            animator.SetBool("Jump", false);
            animator.SetBool("FreeFall", false);

            // Jump
            if(Input.GetKey(KeyCode.Space) && jumpTimeoutDelta <= 0f) {
                ySpeed = Mathf.Sqrt(-jumpHeight * 2 * Physics.gravity.y);
                animator.SetBool("Jump", true);
            }

            // Jump timeout
            if (jumpTimeoutDelta >= 0.0f)
            {
                jumpTimeoutDelta -= Time.deltaTime;
            }

        } else {
            jumpTimeoutDelta = jumpTimeout;

            // Fall timeout
            if (fallTimeoutDelta >= 0.0f) {
                fallTimeoutDelta -= Time.deltaTime;
            } else {
                animator.SetBool("FreeFall", true);
            }

            ySpeed += Physics.gravity.y * Time.deltaTime;
            //ySpeed += -15f * Time.deltaTime;
        }

        Move();

        velocity = moveDir * finalSpeed;
        velocity.y = ySpeed;

        // Player Move
        characterController.Move(velocity * Time.deltaTime);
    }

    private void Move()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // If player run, speed will be change
        finalSpeed = isRun ? runSpeed : moveSpeed;

        // Calculate move vector and direction
        moveInput = (new Vector3(h, 0, v)).normalized;
        moveDir = cameraController.PlanerRotaion * moveInput;

        // If player get input value, player character rotate
        if(moveDir.sqrMagnitude > 0.01f) {
            targetRotation = Quaternion.LookRotation(moveDir);
        }

        // Player character rotate smoothly
        finalRotationSpeed = isRun ? runRotationSpeed : rotationSpeed;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 
            finalRotationSpeed * Time.deltaTime);

        // If player run, character run animation will be play
        float percent = (isRun ? 1f : 0.5f) * moveDir.normalized.magnitude;
        animator.SetFloat("moveAmount", percent, 0.2f, Time.deltaTime);
    }

    private bool IsRun()
    {
        if(Input.GetKey(KeyCode.LeftShift)) {
            isRun = true;
        } else {
            isRun = false;
        }
        return isRun;
    }

    // Use instead of isGrounded 
    private void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius, groundLayer);
        animator.SetBool("Grounded", isGrounded);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0 , 1, 0, 0.5f);
        Gizmos.DrawSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius);
    }

    private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = Random.Range(0, FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(characterController.center), FootstepAudioVolume);
                }
            }
        }

    private void OnLand(AnimationEvent animationEvent)
    {
        // If over 0.5f, Land clip doesn't play land sound
        //if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(characterController.center), FootstepAudioVolume);
        }
    }

    void CheckPoint()
    {
        if(Input.GetKey(KeyCode.R))
        {
            characterController.enabled = false;
            transform.position = new Vector3(checkX, checkY, checkZ);
            //transform.eulerAngles = new Vector3(checkX, checkY, checkZ);
            characterController.enabled = true;
        }
    }

    void LoadCheckPoint()
    {
        checkX = PlayerPrefs.GetFloat("Check X", 0f);
        checkY = PlayerPrefs.GetFloat("Check Y", 0f);
        checkZ = PlayerPrefs.GetFloat("Check Z", 0f);

        characterController.enabled = false;
        transform.position = new Vector3(checkX, checkY, checkZ);
        //transform.eulerAngles = new Vector3(checkX, checkY, checkZ);
        characterController.enabled = true;
    }

    void PauseGame()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            pause.CallMenu();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Reset")
        {
            characterController.enabled = false;
            transform.position = new Vector3(checkX, checkY, checkZ);
            //transform.eulerAngles = new Vector3(checkX, checkY, checkZ);
            characterController.enabled = true;
        }

        if(other.gameObject.tag == "CheckPoint")
        {
            checkX = other.transform.position.x;
            checkY = other.transform.position.y;
            checkZ = other.transform.position.z;
            PlayerPrefs.SetFloat("Check X", checkX);
            PlayerPrefs.SetFloat("Check Y", checkY);
            PlayerPrefs.SetFloat("Check Z", checkZ);
            PlayerPrefs.Save();
            other.gameObject.SetActive(false);
        }
    }

    // When Character Controller Collider Hit
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        /*if(hit.gameObject.tag == "CheckPoint")
        {
            checkX = hit.transform.position.x;
            checkY = hit.transform.position.y;
            checkZ = hit.transform.position.z;
            PlayerPrefs.SetFloat("Check X", checkX);
            PlayerPrefs.SetFloat("Check Y", checkY);
            PlayerPrefs.SetFloat("Check Z", checkZ);
            PlayerPrefs.Save();
        }*/

        if(hit.gameObject.tag == "Moving")
        {
            var hitX = hit.transform.position.x;
            var hitY = hit.transform.position.y;
            var hitZ = hit.transform.position.z;

            Debug.Log("X = " + hitX + "Y = " + hitY + "Z = " + hitZ);
        }
    }
}
