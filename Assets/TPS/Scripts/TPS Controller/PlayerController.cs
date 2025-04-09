using System.Collections;
using StarterAssets;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Timeline;


// Character Controller Value
//     Slope limit: 45
//     Step Offset: 0.3
//     Skin width: 0.02
//     Min Move Distance: 0.001
//     Center: 0, 0.93, 0
//     Radius: 0.2
//     Height: 1.8


public class PlayerController : MonoBehaviour
{
    public AudioClip LandingAudioClip;
    public AudioClip[] FootstepAudioClips;
    [Range(0, 1)] public float FootstepAudioVolume;

    [Header("Speed Settings")]
    [Tooltip("Player move speed")]
    [SerializeField] float moveSpeed = 3f; // 1.5f
    [Tooltip("Player run speed")]
    [SerializeField] float runSpeed = 8f; //4.8f
    [Tooltip("Final speed determine Player speed")]
    [SerializeField] float finalSpeed;

    [Header("Rotation Settings")]
    [Tooltip("Default Player rotation speed")]
    [SerializeField] float rotationSpeed = 700f; // 500f
    [Tooltip("Player Run rotation speed")]
    [SerializeField] float runRotationSpeed = 850; // 500f
    [Tooltip("Final rotation speed determine Player rotation speed")]
    [SerializeField] float finalRotationSpeed;

    [Header("Ground Check Settings")]
    [Tooltip("Ground check radius")]
    [SerializeField] float groundCheckRadius = 0.2f; // default = 0.2f
    [Tooltip("Ground check offset")]
    [SerializeField] Vector3 groundCheckOffset = new Vector3(0, 0.2f, 0f); // default = (0, 0.15, 0.08) (0, 0.1f, 0.04f)
    [Tooltip("Ground check layer")]
    [SerializeField] LayerMask groundLayer; // default = Obstacles

    bool isGrounded = false;
    float ySpeed;

    [Header("Gravity Settings")]
    [Tooltip("If Player in ground")]
    [SerializeField] float groundGravity = -5f; // -0.5f
    [Tooltip("If Player fall")]
    [SerializeField] float fallGravity = -9.81f; // -9.81f

    [Header("Jump Settings")]
    [Tooltip("Player jump height")]
    [SerializeField] float jumpHeight = 2.8f; // Root motion 으로 바꾸니까 점프 높이가 제대로 안되서 그냥 2.2 정도로 줬음

    [Header("Jump Timeout")]
    [Tooltip("Timeout Player can't continuous jump")]
    [SerializeField] float jumpTimeout = 0f;
    float jumpTimeoutDelta;

    [Header("Fall Timeout")]
    [Tooltip("Timeout Player fall")]
    [SerializeField] float fallTimeout = 0.15f; // 0.15f
    float fallTimeoutDelta;

    float resetTimeout = 2f;
    float resetHold = 0f;
    bool resetTriggered = false;

    Vector3 moveInput;
    Vector3 moveDir;
    Vector3 velocity;

    Quaternion targetRotation;

    CameraController cameraController;
    Animator animator;
    CharacterController characterController;

    [Header("Pause")]
    [Tooltip("Pause Object")]
    [SerializeField] OptionManager optionManager;

    // player checkpoint vector
    float checkX;
    float checkY;
    float checkZ;

    bool isRun = false;
    bool isChanged = false;
    bool hasControl = true;

    [Header("Checkpoint Text")]
    [SerializeField] TMP_Text checkPointText;

    void Awake()
    {
        cameraController = Camera.main.GetComponent<CameraController>();
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        // InitializeValue();
    }

    void Start()
    {
        jumpTimeoutDelta = jumpTimeout;
        fallTimeoutDelta = fallTimeout;

        LoadCheckPoint();
    }

    private void Update()
    {
        if (GameManager.instance != null) {
            FootstepAudioVolume = GameManager.sfxVolume;
        } else FootstepAudioVolume = 0.2f;

        /* Switch TPS and FPS */
        // If Camera distance under 0.5f, Player Character object is disabled
        // And it Over 0.5f, Object is Enabled
        if(!GameManager.isTPS && !isChanged) {
            transform.GetChild(0).gameObject.SetActive(false);
            isChanged = true;
        } else if(GameManager.isTPS && isChanged) {
            transform.GetChild(0).gameObject.SetActive(true);
            isChanged = false;
        }

        // Active Parkour Action, It return
        if(!hasControl) 
        {
            animator.SetBool("Jump", false);
            animator.SetBool("FreeFall", false);
            animator.SetBool("Grounded", isGrounded);
            return;
        }

        // Activate Checkpoint
        if(Input.GetKey(KeyCode.R))
        {
            resetHold += Time.deltaTime;
            Debug.Log("reset : " + resetHold);

            if(resetHold >= resetTimeout && !resetTriggered) {
                CheckPoint();
                resetTriggered = true;
            }
        } else{
            resetHold = 0f;
            resetTriggered = false;
        }

        // Is Player Run?
        IsRun();

        // Is Grounded?
        GroundCheck();
        //GroundedCheck();

        // Pause
        PauseGame();

        // Gravity
        GravityAndJump();

        // Player Move
        Move();
    }

    void InitializeValue()
    {

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

        velocity = moveDir * finalSpeed;
        velocity.y = ySpeed;

        characterController.Move(velocity * Time.deltaTime);
    }

    void GravityAndJump()
    {
        if(isGrounded) {
            ySpeed = groundGravity;

            fallTimeoutDelta = fallTimeout;

            animator.SetBool("Jump", false);
            animator.SetBool("FreeFall", false);

            // Jump
            if(Input.GetButtonDown("Jump") && jumpTimeoutDelta <= 0f && hasControl) {
                ySpeed = Mathf.Sqrt(-jumpHeight * 2 * fallGravity);
                // animator.SetBool("Jump", true);
                animator.CrossFade("JumpStart", 0f);
            }

            // Jump timeout
            if (jumpTimeoutDelta >= 0.0f)
            {
                jumpTimeoutDelta -= Time.deltaTime;
                Debug.Log("Jump = " + (jumpTimeoutDelta >= 0.0f));
            } else Debug.Log("Jump = " + (jumpTimeoutDelta < 0.0f));

        } else {
            jumpTimeoutDelta = jumpTimeout;

            // Fall timeout
            if (fallTimeoutDelta >= 0.0f) {
                fallTimeoutDelta -= Time.deltaTime;
            } else {
                animator.SetBool("FreeFall", true);
            }

            ySpeed += fallGravity * Time.deltaTime;
        }
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

    public void SetControl(bool hasControl)
    {
        this.hasControl = hasControl;
        characterController.enabled = hasControl;

        if(!hasControl)
        {
            animator.SetFloat("moveAmount", 0f);
            targetRotation = transform.rotation;
        }
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
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(characterController.center), FootstepAudioVolume);
        }
    }

    void CheckPoint()
    {
        characterController.enabled = false;
        transform.position = new Vector3(checkX, checkY, checkZ);
        //transform.eulerAngles = new Vector3(checkX, checkY, checkZ);
        characterController.enabled = true;
        StartCoroutine(ShowCheckPointActiveText());
    }

    void LoadCheckPoint()
    {
        checkX = PlayerPrefs.GetFloat("Check X", 0f);
        checkY = PlayerPrefs.GetFloat("Check Y", 0f);
        checkZ = PlayerPrefs.GetFloat("Check Z", -25f);

        characterController.enabled = false;
        transform.position = new Vector3(checkX, checkY, checkZ);
        //transform.eulerAngles = new Vector3(checkX, checkY, checkZ);
        characterController.enabled = true;
    }

    void PauseGame()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(!GameManager.isPause) {
                optionManager.CallMenu();
                optionManager.FindSliders();
            } else optionManager.CloseMenu();
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
            checkX = transform.position.x;
            checkY = transform.position.y;
            checkZ = transform.position.z;

            PlayerPrefs.SetFloat("Check X", checkX);
            PlayerPrefs.SetFloat("Check Y", checkY);
            PlayerPrefs.SetFloat("Check Z", checkZ);
            PlayerPrefs.Save();

            other.transform.gameObject.SetActive(false);
            StartCoroutine(ShowCheckPointText());
        }

        if(other.gameObject.tag == "Victory")
        {
            Debug.Log("Victory");
            // Scene Change
        }
    }

    // When Character Controller Collider Hit
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        
    }

    private IEnumerator ShowCheckPointText()
    {
        checkPointText.SetText("Checkpoint Activated!");
        checkPointText.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        checkPointText.gameObject.SetActive(false);
    }
    private IEnumerator ShowCheckPointActiveText()
    {
        checkPointText.SetText("Checkpoint Loaded!");
        checkPointText.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        checkPointText.gameObject.SetActive(false);
    }

    public float RotationSpeed => rotationSpeed;
}
