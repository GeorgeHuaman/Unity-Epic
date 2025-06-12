using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class ChangeSkybox : MonoBehaviour
{
    public GameObject surface;
    public GameObject underWater;


    public Material ocean;
    public Material sky;

    public void ChangeToOcean()
    {
        RenderSettings.skybox = ocean;
        RenderSettings.fog = true;
        underWater.SetActive(true);
        surface.SetActive(false);
    }
    public void ChangeToSky()
    {
        RenderSettings.skybox = sky;
        RenderSettings.fog = false;
        underWater.SetActive(false);
        surface.SetActive(true);
    }
}
