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
    public float rotationSpeed = 50f;

    [Header("Gravedad")]
    public float gravity = -9.81f;
    public Transform cameraTransform;
    public Transform playerBody;

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
        if (!GameManager.Instance.GetBoolCursorLocked())
        {
            return;
        }

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
        float inputX = Input.GetAxis("Horizontal") + joystickDigital.Horizontal;
        float inputZ = Input.GetAxis("Vertical") + joystickDigital.Vertical;

        Vector3 move = playerBody.forward * inputZ + playerBody.right * inputX;
        move.y = 0f;

        float speed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;
        controller.Move(move * speed * Time.deltaTime);

        if (inputX != 0)
        {
            float rotationAmount = inputX * rotationSpeed * Time.deltaTime;
            playerBody.Rotate(Vector3.up * rotationAmount);
        }
    }

    public void Jump()
    {
        if (isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    public void KnockBack(float knockBackForce)
    {
            velocity.y = Mathf.Sqrt(knockBackForce * -2f * gravity);
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
