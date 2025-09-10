using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Urn : MonoBehaviour
{
    public TypeObject.ObjectType urnaType;
    public PickUp pickUp;
    public TMP_Text listObjects;
    public List<GameObject> objects = new List<GameObject>();
    public string nombretipo;
    public int quantityNeedle;
    public Interactable interactable;
    public Interactable interactableFinal;
    private void Start()
    {
        interactableFinal.enabled = false;
    }
    public void TryStoreObject()
    {
        if (pickUp == null)
            return;

        if (pickUp.currentObject != null && pickUp.currentType == urnaType)
        {
            objects.Add(pickUp.currentObject);
            UpdateListText();
            pickUp.ReleaseObject();
            if (objects.Count >= quantityNeedle)
            {
                interactable.enabled = false;
                interactableFinal.enabled = true;
            }
        }
    }

    private void UpdateListText()
    {
        if (listObjects == null)
            return;
        listObjects.text = $"Cofre de {nombretipo}:\n";
        foreach (var obj in objects)
        {
            if (obj != null)
                listObjects.text += $"- {obj.name}\n";
        }
    }
}
