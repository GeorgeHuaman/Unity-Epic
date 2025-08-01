using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ManagerPanelAlumns : MonoBehaviour
{
    public GameObject panelMaster;

    public void ButtonBack()
    {
        if (panelMaster.activeSelf)
        {
           panelMaster.SetActive(false);
        }
    }
}
