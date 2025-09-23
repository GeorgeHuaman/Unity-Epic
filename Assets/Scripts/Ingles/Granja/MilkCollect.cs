using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MilkCollect : MonoBehaviour
{
   public Interactable interactable;
    public void milkCollect()
    {
        interactable.Enable();
        GetComponentInChildren<Interactable>().Destroy() ;
        GameCow.instance.AgreeMilk();
        Destroy(gameObject);
    }
}
