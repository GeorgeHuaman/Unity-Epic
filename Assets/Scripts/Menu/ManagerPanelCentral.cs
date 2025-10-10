using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerPanelCentral : MonoBehaviour
{
    public GameObject central;
    public List<GameObject> temas = new List<GameObject>();

    public void Back()
    {
        if(central.activeSelf)
        {
            central.SetActive(false);
        }
        else
        {
            SearchPanelsActive().SetActive(false);
            central.SetActive(true);
        }
    }

    private GameObject SearchPanelsActive()
    {
        foreach (GameObject go in temas)
        {
            if(go.activeSelf)
                return go;
        }
        return null;
    }
}
