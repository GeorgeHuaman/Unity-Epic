using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleHeight : MonoBehaviour
{
    private RectTransform thisRect;
    [SerializeField]
    private RectTransform targetRectAlumns;
    [SerializeField]
    private RectTransform targetRectPerfil;
    public GameObject panelMaster;

    void Start()
    {
        thisRect = GetComponent<RectTransform>();
        panelMaster.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (targetRectAlumns.gameObject.activeSelf)
        {
            float size = targetRectAlumns.rect.height;
            Vector2 rec = new Vector2(thisRect.sizeDelta.x, targetRectAlumns.sizeDelta.y);
            thisRect.sizeDelta = rec;
        }
        else
        {
            float size = targetRectPerfil.rect.height;
            Vector2 rec = new Vector2(thisRect.sizeDelta.x, targetRectPerfil.sizeDelta.y);
            thisRect.sizeDelta = rec;
        }
    }
}
