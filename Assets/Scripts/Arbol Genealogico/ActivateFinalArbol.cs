using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateFinalArbol : MonoBehaviour
{
    public List<Choose> objects = new List<Choose>();
    public GameObject interactFinal;
    public static ActivateFinalArbol instance;
    private void Start()
    {
        instance = this;
    }
    public void AreAllComplete()
    {
        foreach (Choose obj in objects)
        {
            if (obj == null) continue;

            if (!obj.completed)
                return;
        }

        interactFinal.SetActive(true);
    }
}
