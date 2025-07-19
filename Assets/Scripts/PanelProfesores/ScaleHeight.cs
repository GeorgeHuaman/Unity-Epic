using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleHeight : MonoBehaviour
{
    private RectTransform thisRect;
    [SerializeField]
    private RectTransform targetRect;

    void Start()
    {
        thisRect = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        float size = targetRect.rect.height;
        Vector2 rec = new Vector2(thisRect.sizeDelta.x, targetRect.sizeDelta.y);
        thisRect.sizeDelta = rec;
    }
}
