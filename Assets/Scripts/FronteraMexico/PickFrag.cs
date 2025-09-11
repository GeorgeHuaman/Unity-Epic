using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PickFrag : MonoBehaviour
{
    public TMP_Text objectName;
    public FragType.ObjectType currentType;
    public GameObject currentObject;
    public Interactable interactable;
    public GameObject hand;
    private bool isMoving = false;
    public float moveSpeed = 5f;


    [Header("Offset")]
    public Vector3 offset = new Vector3(0, 0.3f, 0);
    private void Update()
    {
        if (isMoving && currentObject != null)
        {
            currentObject.transform.position = Vector3.Lerp(currentObject.transform.position,
            hand.transform.position + offset,
            Time.deltaTime * moveSpeed);


            if (Vector3.Distance(currentObject.transform.position, hand.transform.position) < 0.05f)
            {
                currentObject.transform.position = hand.transform.position;
            }
        }
    }
    public void Pick(GameObject obj)
    {
        if (currentObject != null)
            return;

        currentObject = obj;
        interactable = currentObject.GetComponentInChildren<Interactable>();
        interactable.enabled = false;
        interactable.interactButton.gameObject.SetActive(false);
        interactable.buttonParent.SetActive(false);
        isMoving = true;
        objectName.text = "Seleccionado: " + obj.name;

        FragType typeObj = obj.GetComponent<FragType>();
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
