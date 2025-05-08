using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ColliderImageInteractuable : MonoBehaviour
{
    void Start()
    {
        RectTransform rt = GetComponent<RectTransform>();
        BoxCollider col = GetComponent<BoxCollider>();
        col.size = new Vector3(rt.rect.width, rt.rect.height, 1f);
        col.center = Vector3.zero;
    }
}
