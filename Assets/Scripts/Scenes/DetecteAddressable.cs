using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetecteAddressable : MonoBehaviour
{
    private VersionInstances version;    
    private void Awake()
    {
        version = VersionInstances.instance;
        version.gameObject = this.gameObject;
    }
}
