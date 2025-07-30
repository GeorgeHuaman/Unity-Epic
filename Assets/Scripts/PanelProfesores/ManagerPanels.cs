using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;

public class ManagerPanels : MonoBehaviour
{
    public GameObject panelInfo;
    public GameObject time;
    public GameObject panelAlumns;
    public GameObject panelMaster;
    public TextMeshProUGUI nameColegio;

    private void Start()
    {
    }
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
                NameColegio();
                panelAlumns.SetActive(true);
            }
        }
    }
    public void NameColegio()
    {
        Debug.Log(UserSession.Instance.NameColegio());
        nameColegio.text = UserSession.Instance.NameColegio();
    }
}
