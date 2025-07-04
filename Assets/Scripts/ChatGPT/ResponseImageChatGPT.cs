using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResponseImageChatGPT : MonoBehaviour
{
    private RectTransform rectTransform;
    public RectTransform copyTransform;
    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        CopyRectTransformValues(rectTransform, copyTransform);
    }
    public static void CopyRectTransformValues(RectTransform target, RectTransform source)
    {
        if (target == null || source == null)
        {
            Debug.LogWarning("Uno de los RectTransform es nulo.");
            return;
        }

        target.SetParent(source.parent, true); // Mantiene valores locales

        target.anchorMin = source.anchorMin;
        target.anchorMax = source.anchorMax;
        target.pivot = source.pivot;
        target.anchoredPosition = source.anchoredPosition;
        target.sizeDelta = source.sizeDelta;

        target.localRotation = source.localRotation;
        target.localScale = source.localScale;
        target.localPosition = source.localPosition;
    }
}
