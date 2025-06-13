using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockbackOnImpact : MonoBehaviour
{

    private bool canKnockBack = true;
    private bool canJump = true;
    private float cooldownTime = 0.2f;

    public void KnockBack()
    {
        if (canKnockBack)
        {
            //SpatialBridge.actorService.localActor.avatar.AddForce(new Vector3(0f, 10f, 0f));
            StartCoroutine(KnockBackCooldown());
        }
    }

    public void Jump()
    {
        if (canJump)
        {
            //SpatialBridge.actorService.localActor.avatar.AddForce(new Vector3(0f, 20f, 0f));
            StartCoroutine(JumpCooldown());
        }
    }

    private IEnumerator KnockBackCooldown()
    {
        canKnockBack = false;
        yield return new WaitForSeconds(cooldownTime);
        canKnockBack = true;
    }

    private IEnumerator JumpCooldown()
    {
        canJump = false;
        yield return new WaitForSeconds(cooldownTime);
        canJump = true;
    }
}
