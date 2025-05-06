using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
public class CameraLook : MonoBehaviour
{
    float xMove;
    float yMove;
    float xRotation = 0f;
    public Transform playerBody;
    public float sensitivity = 2f;
    public float sensitivityMovil = 40f;
    
    public Vector2 LockAxis;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    void Update()
    {
        
        //if (IsOnMobile())
            CameraMovil();
        //else
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
        xMove = LockAxis.x * sensitivityMovil * Time.deltaTime;
        yMove = LockAxis.y * sensitivityMovil * Time.deltaTime;
        xRotation -= yMove;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        playerBody.Rotate(Vector3.up * xMove);
    }
    bool IsOnMobile()
    {
        return Application.platform == RuntimePlatform.Android ||
               Application.platform == RuntimePlatform.IPhonePlayer;
    }
}
