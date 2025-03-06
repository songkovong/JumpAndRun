using Unity.Burst.Intrinsics;
using UnityEditor.Rendering.Universal;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class PlayerMovement : MonoBehaviour
{
    Animator _animator;
    Camera _camera;
    CharacterController _controller;

    public float speed = 5f;
    public float runSpeed = 8f;
    public float finalSpeed;
    public bool isRun;
    public bool toggleCameraRotation;
    public float smoothness = 10f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _animator = this.GetComponent<Animator>();
        _camera = Camera.main;
        _controller = this.GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.LeftAlt)) { 
            toggleCameraRotation = true; // 둘러보기 활성화
        } else {
            toggleCameraRotation = false; // 둘러보기 비활성화
        }

        if(Input.GetKey(KeyCode.LeftShift)) {
            isRun = true;
        } else {
            isRun = false;
        }
        
        InputMovement();
    }

    void LateUpdate()
    {
        if(toggleCameraRotation != true)
        {
            Vector3 playerRotate = Vector3.Scale(_camera.transform.forward, new Vector3(1, 0, 1));
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(playerRotate), smoothness * Time.deltaTime);
        }
    }

    void InputMovement()
    {
        finalSpeed = !(isRun) ? speed : runSpeed;

        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right); 

        //Vector3 forward = _camera.transform.forward;
        //Vector3 right = _camera.transform.right;

        Vector3 moveDirection = forward * Input.GetAxisRaw("Vertical") + right * Input.GetAxisRaw("Horizontal");
        //moveDirection.y = 0;
        //Vector3 finalMoveDirection = Vector3.Scale(moveDirection, new Vector3(1, 0, 1));
    
        _controller.Move(moveDirection.normalized * finalSpeed * Time.deltaTime);
        //transform.rotation = Quaternion.LookRotation(moveDirection);

        float percent = ((isRun) ? 1f : 0.5f) * moveDirection.normalized.magnitude;
        _animator.SetFloat("Blend", percent, 0.1f, Time.deltaTime);
    }
}
