using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PickFragFuture : MonoBehaviour
{
    public TMP_Text objectName;
    public FragTypeFuture.ObjectType currentType;
    public GameObject currentObject;
    public Interactable interactable;
    private bool isMoving = false;
    public float moveSpeed = 5f;


    public void Pick(GameObject obj)
    {
        if (currentObject != null)
            return;

        currentObject = obj;
        interactable = currentObject.GetComponentInChildren<Interactable>();
        interactable.Disable();
        isMoving = true;
        objectName.text = "Seleccionado: " + obj.name;

        FragTypeFuture typeObj = obj.GetComponent<FragTypeFuture>();
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
            objectName.text = "";
            isMoving = false;
        }
    }
}
