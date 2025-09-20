using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Pick : MonoBehaviour
{
    public ObjectType.Type currentType;
    public GameObject currentObject;
    public Interactable interactable;
    private bool isMoving = false;
    public float moveSpeed = 5f;
    public GameObject player;
    public Camera camera;

    [Header("Offset")]
    public Vector3 offset = new Vector3(0, 0.5f, 0);

    private void Update()
    {
        if (currentObject != null)
        {
            Vector3 targetPos = player.transform.position + offset;

            currentObject.transform.position = Vector3.Lerp(
                currentObject.transform.position,
                targetPos,
                Time.deltaTime * moveSpeed
            );

            //if (camera != null )
            //{
            //    currentObject.transform.LookAt(camera.transform.position);

            //    currentObject.transform.Rotate(0, 180f, 0);
            //}

            // Ajuste final si está muy cerca
            if (Vector3.Distance(currentObject.transform.position, targetPos) < 0.05f)
            {
                currentObject.transform.position = targetPos;
                moveSpeed = 20f;
            }
        }
    }
    public void PickUp(GameObject obj)
    {
        if (currentObject != null)
            return;

        currentObject = obj;
        obj.SetActive(true);
        isMoving = true;
        ObjectType typeObj = obj.GetComponent<ObjectType>();
        interactable = typeObj.interactable;
        interactable.enabled = false;
        interactable.Disable();
        if (typeObj != null)
        {
            currentType = typeObj.type;
            Debug.Log($"Agarraste un {currentType}: {obj.name}");
        }
        else
        {
            Debug.LogWarning("El objeto no tiene TypeObject asignado");
        }
    }
    public void Release()
    {
        if (currentObject != null)
        {
            currentObject = null;
            isMoving = false;
        }
    }
}
