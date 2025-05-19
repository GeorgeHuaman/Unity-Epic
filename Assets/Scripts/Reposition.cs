using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Reposition : MonoBehaviour
{
    public GameObject respawn;
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.CompareTag("Player"))
        {
            transform.position = respawn.transform.position;
        }
    }
}
