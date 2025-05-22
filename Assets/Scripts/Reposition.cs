using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Reposition : MonoBehaviour
{
    public GameObject respawn;
    public GameObject player;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player.GetComponent<CharacterController>().enabled = false;
            player.transform.position = respawn.transform.position;
            player.GetComponent<CharacterController>().enabled = true;
        }
    }
}
