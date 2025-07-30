using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerPanels : MonoBehaviour
{
    public GameObject panelInfo;
    public GameObject time;
    public GameObject panelAlumns;
    public GameObject panelMaster;

    public void ButtonBack()
    {
        if(panelMaster.activeSelf)
        {
            if(panelAlumns.activeSelf) 
            {
                panelMaster.SetActive(false);
            }
            if(panelInfo.activeSelf)
            {
                panelInfo.SetActive(false);
                time.SetActive(false);
                panelAlumns.SetActive(true);
            }
        }
    }
}
