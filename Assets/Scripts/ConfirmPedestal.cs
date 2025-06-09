using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfirmPedestal : MonoBehaviour
{
    public bool red;
    public bool green;
    public bool blue;
    public bool yellow;
    public GameObject tp;
    public static ConfirmPedestal instance;

    private void Awake()
    {
        instance = this;
    }
    public void Red()
    {
        red = true; Telepor();
    }
    public void Green()
    {
        green = true; Telepor();
    }

    public void Blue()
    {
        blue = true; Telepor();
    }
    public void Yellow()
    {
        yellow = true; Telepor();
    }

    public void Telepor()
    {
        if (red && green && yellow && blue)
        {
            tp.SetActive(true);
        }
    }
}
