using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    const string PLAYER_MOVEMENT = "Movement";
    const string PLAYER_JUMP = "Jump";

    [Header("Speed Settings")]
    [SerializeField] float moveSpeed = 3f;
    [SerializeField] float runSpeed = 7f;
    [SerializeField] float finalSpeed;
    [SerializeField] float currentSpeed;

    [Header("Rotation Settings")]
    [SerializeField] float rotationSpeed = 500f;
    [SerializeField] float runRotationSpeed = 500f;
    [SerializeField] float finalRotationSpeed;
    //[SerializeField] float jumpSpeed = 5f;

    [Header("Ground Check Settings")]
    [SerializeField] float groundCheckRadius = 0.2f;
    [SerializeField] Vector3 groundCheckOffset;
    [SerializeField] LayerMask groundLayer;

    bool isGrounded;

    float ySpeed;

    [Header("Jump Settings")]
    [SerializeField] float jumpHeight = 1f;

    [Header("State")]
    [SerializeField] string currentState;

    Vector3 moveInput;
    Vector3 moveDir;
    Vector3 velocity;


    Quaternion targetRotation;

    CameraController cameraController;
    Animator animator;
    CharacterController characterController;

    bool isMove = false;
    bool isRun = false;
    //bool isFalling = false;
    //bool isJumping = false;
    bool canJump = true;
    float jumpCooldown = 0.5f;
    
    void Awake()
    {
        cameraController = Camera.main.GetComponent<CameraController>();
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        currentState = PLAYER_MOVEMENT;
    }

    private void Update()
    {
        // Is Player Run?
        IsRun();

        // Move amount clamp 0 to 1
        //float moveAmount = Mathf.Clamp01(Mathf.Abs(h) + Mathf.Abs(v));

        // Is Grounded?
        GroundCheck();
        Debug.Log("IsGrounded = " + isGrounded);
        //Debug.Log("IsFalling = " + isFalling);
        Debug.Log("currentState = " + currentState);

        // Gravity
        if(isGrounded) {
            ySpeed = -0.5f;

            if(currentState == PLAYER_JUMP)
            {
                /*if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && animator.IsInTransition(0))
                {
                    ChangeAnimState(PLAYER_MOVEMENT);
                }*/
                ChangeAnimState(PLAYER_MOVEMENT);
            }

            if(currentState == PLAYER_MOVEMENT)
            {
                if(Input.GetKey(KeyCode.Space) && canJump)
                {
                    ySpeed = Mathf.Sqrt(-jumpHeight * 2 * Physics.gravity.y);
                    //StartCoroutine(JumpCooldown());
                    ChangeAnimState(PLAYER_JUMP);
                }
            }

            //isFalling = false;
            //isJumping = false;

            // If Player is Grounded, Play animator "Movement" Blend
            //animator.Play("Movement");

        } else {
            //ySpeed += Physics.gravity.y * Time.deltaTime;
            ySpeed += Physics.gravity.y * Time.deltaTime;

            //isFalling = true;

            // If Jumping or Falling, Speed is fixed
        }

        if(currentState == PLAYER_MOVEMENT || currentState == PLAYER_JUMP) {
            Move();
        }

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

        if(currentState == PLAYER_MOVEMENT) {
            currentSpeed = finalSpeed;
        } 
        else if(currentState == PLAYER_JUMP) {
            finalSpeed = currentSpeed;
        }

        // If player get input value, player character rotate
        if(moveDir.sqrMagnitude > 0.01f)
        {
            targetRotation = Quaternion.LookRotation(moveDir);
            isMove = true;
        } else {
            isMove = false;
        }
        // Player character rotate smoothly
        finalRotationSpeed = isRun ? runRotationSpeed : rotationSpeed;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 
            finalRotationSpeed * Time.deltaTime);

        // If player run, character run animation will be play
        float percent = (isRun ? 1f : 0.5f) * moveDir.normalized.magnitude;
        animator.SetFloat("moveAmount", percent, 0.2f, Time.deltaTime);
        //animator.SetBool("isFalling", isFalling);
        float jumpPer = isMove ? 1f : 0f;
        animator.SetFloat("Jump", jumpPer, 0f, Time.deltaTime);
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
        //animator.SetBool("isGround", isGrounded);
    }

    // Jump
    private void Jump()
    {
        if(Input.GetKey(KeyCode.Space) && isGrounded) {
            ySpeed = Mathf.Sqrt(-jumpHeight * 2 * Physics.gravity.y);
            //isJumping = true;
        }
    }

    private void ChangeAnimState(string newState)
    {
        if (currentState == newState) return;

        animator.Play(newState);
        currentState = newState;
    }

    IEnumerator JumpCooldown()
    {
        canJump = false;
        yield return new WaitForSeconds(jumpCooldown);
        canJump = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0 , 1, 0, 0.5f);
        Gizmos.DrawSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius);
    }
}
