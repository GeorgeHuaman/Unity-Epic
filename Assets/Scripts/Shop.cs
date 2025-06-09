using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public AudioClip buy;

    public RawImage imageItem;

    public GameObject grid;

    public GameObject door;

    public List<Texture2D> listTexture = new List<Texture2D>();

    public bool firtsItem, secondItem, theerItem;


    public void Buy(Texture2D image)
    {
        bool agree = true;
        for (int i = 0; i < listTexture.Count; i++)
        {
            if (listTexture[i] == image)
            {
                agree = false;
                break;
            }
        }
        if (agree)
        {
            listTexture.Add(image);
            RawImage a = imageItem;
            a.texture = image;
            Instantiate(a, new Vector3(0, 0, 0), Quaternion.identity, grid.transform);
        }
    }

    #region No ver
    public void FirstItem()
    {
        firtsItem = true;
        OpenDoor();
    }
    public void SecondItem()
    {
        secondItem = true;
        OpenDoor();
    }
    public void TheerItem()
    {
        theerItem = true;
        OpenDoor();
    }
    public void OpenDoor()
    {
        if(firtsItem && secondItem &&  theerItem) 
        {
            door.SetActive(true);
        }
    }
    #endregion
}
