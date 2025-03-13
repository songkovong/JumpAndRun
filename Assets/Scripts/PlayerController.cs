using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Speed Settings")]
    [SerializeField] float moveSpeed = 3f;
    [SerializeField] float runSpeed = 7f;
    [SerializeField] float finalSpeed;

    [Header("Rotation Settings")]
    [SerializeField] float rotationSpeed = 500f;
    [SerializeField] float runRotationSpeed = 500f;
    [SerializeField] float finalRotationSpeed;

    [Header("Ground Check Settings")]
    [SerializeField] float groundCheckRadius = 0.15f;
    [SerializeField] Vector3 groundCheckOffset;
    [SerializeField] LayerMask groundLayer;

    bool isGrounded;

    float ySpeed;

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
    }

    private void Update()
    {
        // Is Player Run?
        IsRun();

        // Is Grounded?
        GroundCheck();

        // Gravity
        if(isGrounded) {
            ySpeed = -0.5f;

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
}
