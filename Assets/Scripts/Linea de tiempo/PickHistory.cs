using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PickHistory : MonoBehaviour
{
    public TMP_Text objectName;
    public HistoryType.ObjectType currentType;
    public GameObject currentObject;
    public Interactable interactable;
    private bool isMoving = false;
    public float moveSpeed = 5f;


    public void Pick(GameObject obj)
    {
        if (currentObject != null)
            return;

        HistoryType typeObj = obj.GetComponent<HistoryType>();
        currentObject = obj;
        interactable = currentObject.GetComponent<Interactable>();
        interactable.Disable();
        isMoving = true;

        objectName.text = "Seleccionado: " + typeObj.letter.name;
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
