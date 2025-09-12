using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PickUp : MonoBehaviour
{
    public TMP_Text objectName;
    public TypeObject.ObjectType currentType; 
    public GameObject currentObject;
    public Interactable interactable;

    public void PickUpObject(GameObject obj)
    {
        if (currentObject != null)
            return;

        currentObject = obj;
        interactable = currentObject.GetComponentInChildren<Interactable>();
        interactable.Disable();
        objectName.text = "Seleccionado: " + obj.name;

        TypeObject typeObj = obj.GetComponentInChildren<TypeObject>();
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

    public void ReleaseObject()
    {
        currentObject = null;
        objectName.text = string.Empty;
        interactable = null;
    }



}
