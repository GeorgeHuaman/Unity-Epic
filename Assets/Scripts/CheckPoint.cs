using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public Transform point;
    public GameObject player;
    
    public void ResetPosition()
    {
        player.GetComponent<CharacterController>().enabled = false;
        player.transform.position = point.transform.position;
        player.GetComponent<CharacterController>().enabled = true;
    }

    public void ObtainTransform(Transform pos)
    {
        point = pos.transform;
    }
}
