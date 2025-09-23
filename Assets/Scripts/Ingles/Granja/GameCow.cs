using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameCow : MonoBehaviour
{
    public TextMeshProUGUI milkText;
    public int milkCount;
    public static GameCow instance;
    public GameObject prefabMilk;
    private GameObject objectMilk;
    // Start is called before the first frame update
    void Start()
    {
     instance = this;   
    }

    public void GeneratedMilk(GameObject position)
    {
        objectMilk = Instantiate(prefabMilk, position.transform.position, Quaternion.identity);
    }
    public void CowInteractuable(Interactable interactable)
    {
        objectMilk.GetComponent<MilkCollect>().interactable = interactable;
        interactable.Disable();
    }
    public void AgreeMilk()
    {
        milkCount++;
        UpdateText();
    }

    public void UpdateText()
    {
        milkText.text = $"{milkCount}";
    }
}
