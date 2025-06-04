using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasMobileManager : MonoBehaviour
{
    public GameObject canvasMobile;
    void Start()
    {
        canvasMobile.SetActive(IsOnMobile()); 
    }


    bool IsOnMobile()
    {
        return Application.platform == RuntimePlatform.Android ||
               Application.platform == RuntimePlatform.IPhonePlayer;
    }
}
