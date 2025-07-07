using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatGPTAnims : MonoBehaviour
{
    public Animator anim;

    public void StarTalking()
    {
        anim.SetTrigger("Talk");
    }

    public void Walk()
    {
        anim.SetBool("Walk", true);
    }
}
