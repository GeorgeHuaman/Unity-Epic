using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageInteractuable : MonoBehaviour
{
    [HideInInspector]public string name;
    // Start is called before the first frame update
    void Start()
    {
        name = gameObject.name;
    }

    public void OpenUI()
    {
        ImageUi.instance.OpenWordUi(name);
    }
}
