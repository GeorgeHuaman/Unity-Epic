using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class HibridCharacterController : MonoBehaviour
{
    [Header("Movimiento")]
    public float walkSpeed = 2f;
    public float sprintSpeed = 4f;
    public float jumpHeight = 1.2f;

    [Header("Gravedad")]
    public float gravity = -9.81f;
    public Transform cameraTransform;

    [Header("Modo")]
    public bool forceNonVR = false;

    CharacterController controller;
    Vector3 velocity;
    bool isGrounded;

    public Joystick joystickDigital;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        if (IsUsingVR()) return;

        HandleGroundCheck();
        Move();
        HandleInputs();
        Gravity();
    }
    
    bool IsOnMobile()
    {
        return Application.platform == RuntimePlatform.Android ||
               Application.platform == RuntimePlatform.IPhonePlayer;
    }
    bool IsUsingVR()
    {
        return XRSettings.isDeviceActive && !forceNonVR;
    }
    void HandleGroundCheck()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;
    }
    private void Move()
    {
        if (velocity.x < 0)
        {

        }
        float inputX = Input.GetAxis("Horizontal");
        float inputZ = Input.GetAxis("Vertical");

        inputX += joystickDigital.Horizontal;
        inputZ += joystickDigital.Vertical;

        Vector3 move = cameraTransform.right * inputX + cameraTransform.forward * inputZ;
        move.y = 0f;

        float speed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;
        controller.Move(move * speed * Time.deltaTime);
    }
    public void Jump()
    {
        if (isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        
    }
    private void Gravity()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void HandleInputs()
    {

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            forceNonVR = !forceNonVR;
        }
    }
}
