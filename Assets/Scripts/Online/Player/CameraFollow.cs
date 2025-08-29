using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
   public static CameraFollow singleton
   {
        get => _singleton;
        set
        {
            if (value == null)
                _singleton = null;
            else if(_singleton == null)
                _singleton = value;
            else if(_singleton != value)
            {
                Destroy(value);
            }
        }
   }

    private static CameraFollow _singleton;
    private Transform target;

    private void Awake()
    {
        singleton = this;
    }

    private void OnDestroy()
    {
        if(singleton == this)
            singleton = null;
    }

    private void LateUpdate()
    {
        if(target != null)
            transform.SetPositionAndRotation(target.position, target.rotation);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
