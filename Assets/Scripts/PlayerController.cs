using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
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

    Quaternion targetRotation;

    CameraController cameraController;
    Animator animator;
    CharacterController characterController;

    bool isRun = false;
    bool isFalling = false;
    bool isJumping = false;



    void Awake()
    {
        cameraController = Camera.main.GetComponent<CameraController>();
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        // Is Player Run?
        IsRun();

        // Move amount clamp 0 to 1
        float moveAmount = Mathf.Clamp01(Mathf.Abs(h) + Mathf.Abs(v));

        // If player run, speed will be change
        finalSpeed = isRun ? runSpeed : moveSpeed;

        // Calculate move vector and direction
        var moveInput = (new Vector3(h, 0, v)).normalized;
        var moveDir = cameraController.PlanerRotaion * moveInput;

        // Is Grounded?
        GroundCheck();
        Debug.Log("IsGrounded = " + isGrounded);
        Debug.Log("IsFalling = " + isFalling);

        // Gravity
        if(isGrounded) {
            ySpeed = -0.5f;
            currentSpeed = finalSpeed;

            isFalling = false;
            isJumping = false;

            // If Player is Grounded, Play animator "Movement" Blend
            //animator.Play("Movement");

        } else {
            //ySpeed += Physics.gravity.y * Time.deltaTime;
            ySpeed += -15f * Time.deltaTime;

            isFalling = true;

            // If Jumping or Falling, Speed is fixed
            finalSpeed = currentSpeed;
        }

        Jump();

        var velocity = moveDir * finalSpeed;
        velocity.y = ySpeed;

        // Player Move
        characterController.Move(velocity * Time.deltaTime);

        // If player get input value, player character rotate
        if(moveAmount > 0)
        {
            targetRotation = Quaternion.LookRotation(moveDir);
        }
        // Player character rotate smoothly
        finalRotationSpeed = isRun ? runRotationSpeed : rotationSpeed;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 
            finalRotationSpeed * Time.deltaTime);

        // If player run, character run animation will be play
        float percent = (isRun ? 1f : 0.5f) * moveAmount;
        animator.SetFloat("moveAmount", percent, 0.2f, Time.deltaTime);
        animator.SetBool("isFalling", isFalling);
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
            isJumping = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0 , 1, 0, 0.5f);
        Gizmos.DrawSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius);
    }
}
