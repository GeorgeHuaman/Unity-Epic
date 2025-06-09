using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
public class CameraLook : MonoBehaviour
{
    [Header("Pc")]
    float xMove;
    float yMove;
    float xRotation = 0f;
    public float sensitivity = 2f;

    [Header("Movil")]
    float rotateH;
    float rotateV;
    public float speedGiro = 0.2f;
    public Joystick joystickDigital;

    private float verticalRotation = 0f;

    public Transform playerBody;

    
    

    void Start()
    {

    }

    void Update()
    {
        if (GameManager.Instance.IsCanvasOpen())
        {
            Cursor.lockState = CursorLockMode.None;
            return;
        }
        else Cursor.lockState = CursorLockMode.Locked;



        if (IsOnMobile())
            CameraMovil();
        else
            CameraMouse();

    }

    private void CameraMouse()
    {
        xMove = Input.GetAxis("Mouse X") * sensitivity;
        yMove = Input.GetAxis("Mouse Y") * sensitivity;

        xRotation -= yMove;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * xMove);
    }

    private void CameraMovil()
    {
        rotateH = joystickDigital.Horizontal * speedGiro;
        float rotateV = -(joystickDigital.Vertical * speedGiro);

        // Acumular la rotación vertical
        verticalRotation += rotateV;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

        // Aplicar la rotación vertical con restricción
        transform.localEulerAngles = new Vector3(verticalRotation, 0, 0);

        // Rotación horizontal sin restricción
        playerBody.Rotate(0, rotateH, 0);
    }
    bool IsOnMobile()
    {
        return Application.platform == RuntimePlatform.Android ||
               Application.platform == RuntimePlatform.IPhonePlayer;
    }
}
