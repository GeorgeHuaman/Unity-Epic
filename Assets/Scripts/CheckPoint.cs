using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public Transform point;
    
    public void ResetPosition()
    {
        //SpatialBridge.actorService.localActor.avatar.position = point.position;
    }

    public void ObtainTransform(Transform pos)
    {
        point = pos.transform;
    }
}
