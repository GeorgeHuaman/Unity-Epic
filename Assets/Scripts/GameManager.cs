using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private bool isCanvasOpen;
    void Start()
    {
        Instance = this;
    }

    public bool IsCanvasOpen()
    {
        return isCanvasOpen;
    }

    public void SetIsCanvasOpen(bool set)
    {
        isCanvasOpen = set;
    }
}
