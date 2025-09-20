using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggCollect : MonoBehaviour
{
    public void Agree()
    {
        GameChicken.instance.AgreeEggs();
        Destroy(gameObject);
    }
}
