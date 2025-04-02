using System.Collections;
using StarterAssets;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

/*
********************************************************************
Character Controller Value

    Slope limit: 45
    Step Offset: 0.3
    Skin width: 0.02
    Min Move Distance: 0.001
    Center: 0, 0.93, 0
    Radius: 0.2
    Height: 1.8

********************************************************************
*/

public class PlayerController : MonoBehaviour
{
    public AudioClip LandingAudioClip;
    public AudioClip[] FootstepAudioClips;
    [Range(0, 1)] public float FootstepAudioVolume;

    [Header("Speed Settings")]
    [SerializeField] float moveSpeed = 1.5f;
    [SerializeField] float runSpeed = 4.8f;
    [SerializeField] float finalSpeed;

    [Header("Rotation Settings")]
    [SerializeField] float rotationSpeed = 500f;
    [SerializeField] float runRotationSpeed = 500f;
    [SerializeField] float finalRotationSpeed;

    [Header("Ground Check Settings")]
    [SerializeField] float groundCheckRadius = 0.19f; // default = 0.2f
    [SerializeField] Vector3 groundCheckOffset = new Vector3(0, 0.1f, 0f); // default = (0, 0.15, 0.08) (0, 0.1f, 0.04f)
    [SerializeField] LayerMask groundLayer; // default = Obstacles

    [Tooltip("Useful for rough ground")]
    public float GroundedOffset = -0.18f; // -0.14f

    [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
    public float GroundedRadius = 0.19f; // 0.28f

    [Tooltip("What layers the character uses as ground")]
    public LayerMask GroundLayers; // Obstacles

    bool isGrounded = false;

    float ySpeed;
    [SerializeField] float groundGravity = -5f; // -0.5f
    [SerializeField] float fallGravity = -9.81f; // -9.81f

    [Header("Jump Settings")]
    [SerializeField] float jumpHeight = 1f;

    [Header("Jump Timeout")]
    [SerializeField] float jumpTimeout = 0.5f;
    float jumpTimeoutDelta;

    [Header("Fall Timeout")]
    [SerializeField] float fallTimeout = 0.15f; // 0.15f
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

    public TMP_Text checkPointTxt;


    StarterAssetsInputs input;



    bool isRun = false;
    bool isChanged = false;

    void Awake()
    {
        cameraController = Camera.main.GetComponent<CameraController>();
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        input = GetComponent<StarterAssetsInputs>();
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

        // check
        CheckPoint();

        // Is Player Run?
        IsRun();

        // Is Grounded?
        GroundCheck();
        //GroundedCheck();

        // Pause
        PauseGame();

        // Gravity
        /*if(isGrounded) {
            ySpeed = groundGravity;

            fallTimeoutDelta = fallTimeout;

            animator.SetBool("Jump", false);
            animator.SetBool("FreeFall", false);

            // Jump
            if(Input.GetButtonDown("Jump") && jumpTimeoutDelta <= 0f) {
                ySpeed = Mathf.Sqrt(-jumpHeight * 2 * fallGravity);
                //animator.SetBool("Jump", true);
                animator.CrossFade("JumpStart", 0f);
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

            ySpeed += fallGravity * Time.deltaTime;
            //ySpeed += -15f * Time.deltaTime;
        }*/
        GravityAndJump();

        // Player Move
        Move();
        /*velocity = moveDir * finalSpeed;
        velocity.y = ySpeed;

        characterController.Move(velocity * Time.deltaTime);*/
        
        Debug.Log(isGrounded);
}

    private void Move()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        //float h = input.move.x;
        //float v = input.move.y;

        // If player run, speed will be change
        finalSpeed = isRun ? runSpeed : moveSpeed;
        //finalSpeed = input.sprint ? runSpeed : moveSpeed;

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
            if(Input.GetButtonDown("Jump") && jumpTimeoutDelta <= 0f) {
            //if(input.jump && jumpTimeoutDelta <= 0f) {
                ySpeed = Mathf.Sqrt(-jumpHeight * 2 * fallGravity);
                animator.SetBool("Jump", true);
                //animator.CrossFade("JumpStart", 0f);
                Debug.Log("Jump");
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

            //input.jump = false;

            ySpeed += fallGravity * Time.deltaTime;
            //ySpeed += -15f * Time.deltaTime;
        }
    }

    private bool IsRun()
    {
        if(Input.GetKey(KeyCode.LeftShift)) {
        //if(input.sprint) {
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

    /*private void GroundedCheck()
    {
        // set sphere position, with offset
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
            transform.position.z);
        isGrounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
            QueryTriggerInteraction.Ignore);

        // update animator if using character
        animator.SetBool("Grounded", isGrounded);
    }*/

    /*private void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (isGrounded) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;

        // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
        Gizmos.DrawSphere(
            new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
            GroundedRadius);
    }*/

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
            if(!GameManager.isPause) {
                pause.CallMenu();
            }
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

            //other.gameObject.SetActive(false);
            other.transform.parent.gameObject.SetActive(false);

            //StartCoroutine(CheckPointTxt());
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

    IEnumerator CheckPointTxt()
    {
        checkPointTxt.enabled = true;
        yield return new WaitForSeconds(3f);
        checkPointTxt.enabled = false;
    }
}
