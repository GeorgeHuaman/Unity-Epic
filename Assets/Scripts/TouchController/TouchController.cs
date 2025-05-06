using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchController : MonoBehaviour
{
    public FixedTouchField fixedTouchField;
    public CameraLook cameraLook;
    void Start()
    {
        
    }

    
    void Update()
    {
        cameraLook.LockAxis = fixedTouchField.TouchDist;
    }
}
