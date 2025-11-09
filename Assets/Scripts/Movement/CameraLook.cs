using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    private void Start()
    {
        // Buscar joystick si no está asignado
        if (joystickDigital == null)
            joystickDigital = FindInactiveObjectByName<Joystick>("Fixed Joystick Rotate");
    }
    void Update()
    {
        if (!GameManager.Instance.GetBoolCursorLocked())
        {
            return;
        }

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

    T FindInactiveObjectByName<T>(string name) where T : Component
    {
        Scene scene = SceneManager.GetActiveScene();
        GameObject[] rootObjects = scene.GetRootGameObjects();

        foreach (GameObject root in rootObjects)
        {
            T found = FindInChildren<T>(root.transform, name);
            if (found != null)
                return found;
        }

        return null;
    }
    T FindInChildren<T>(Transform parent, string name) where T : Component
    {
        if (parent.name == name && parent.TryGetComponent(out T comp))
            return comp;

        foreach (Transform child in parent)
        {
            T result = FindInChildren<T>(child, name);
            if (result != null)
                return result;
        }

        return null;
    }
}
