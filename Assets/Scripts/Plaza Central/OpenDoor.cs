using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    public Animator animator;
    private bool open;
    public void InteractWithDoor()
    {
        open = !open;
        animator.SetBool("open", open);
    }
}
