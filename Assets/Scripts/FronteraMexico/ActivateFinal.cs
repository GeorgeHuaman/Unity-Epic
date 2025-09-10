using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateFinal : MonoBehaviour
{
    public List<FragPos> objects = new List<FragPos>();
    public GameObject interactFinal;
    public void AreAllComplete()
    {
        foreach (FragPos obj in objects)
        {
            if (obj == null) continue;

            if (!obj.completed)
                return;
        }

        interactFinal.SetActive(true);
    }
}
