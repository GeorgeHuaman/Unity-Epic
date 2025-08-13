using System.Collections;
using System.Collections.Generic;
using Google.Apis.Sheets.v4.Data;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
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
    public LayerMask grounded;

    public Joystick joystickDigital;
    public Button buttonJump;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (buttonJump != null )
        {
            buttonJump.onClick.AddListener(Jump);
        }
        

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
        //else { IsOnPC(); }

        HandleGroundCheck();
        Move();
        HandleInputs();
        Gravity();
    }

    void IsOnPC()
    {
        CharacterController controller = GetComponent<CharacterController>();
        if (controller.enabled)
        {
            this.gameObject.AddComponent<Rigidbody>();
            GetComponent<Rigidbody>().freezeRotation = true;
            this.gameObject.AddComponent<CapsuleCollider>();
        }
            controller.enabled = false;
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
        if (GetComponent<CharacterController>().enabled)
        {
            isGrounded = controller.isGrounded;
        }
        else
        {
            isGrounded = Physics.Raycast(this.gameObject.transform.position, Vector3.down, 0.3f, grounded);
        }
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;
    }
    private void Move()
    {
        if (GetComponent<CharacterController>().enabled)
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
        else
        {
            Rigidbody rb = GetComponent<Rigidbody>();

            float inputX = Input.GetAxis("Horizontal") + joystickDigital.Horizontal;
            float inputZ = Input.GetAxis("Vertical") + joystickDigital.Vertical;

            velocity = rb.velocity;

            velocity.x = (playerBody.forward * inputZ + playerBody.right * inputX).x * walkSpeed;
            velocity.z = (playerBody.forward * inputZ + playerBody.right * inputX).z * walkSpeed;
            velocity.y += gravity * Time.deltaTime;


            rb.velocity = velocity;
        }
    }

    public void Jump()
    {
        if (isGrounded)
        {
            if (GetComponent<CharacterController>().enabled)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
            else
            {
                float jumpForce = Mathf.Sqrt(jumpHeight * -2f * gravity);

                Rigidbody rb = GetComponent<Rigidbody>();
                velocity = rb.velocity;
                velocity.y = jumpForce;
                rb.velocity = velocity;
            }
        }
    }

    public void KnockBack(float knockBackForce)
    {
        velocity.y = Mathf.Sqrt(knockBackForce * -2f * gravity);
    }

    private void Gravity()
    {
        velocity.y += gravity * Time.deltaTime;
        if (GetComponent<CharacterController>().enabled)
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
