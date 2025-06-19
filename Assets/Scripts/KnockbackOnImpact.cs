using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class KnockbackOnImpact : MonoBehaviour
{
    public HibridCharacterController characterController;
    public float knockBackForce;
    private bool canKnockBack = true;
    private float cooldownTime = 0.25f;
    public void KnockBack()
    {
        if (canKnockBack)
        {
            characterController.KnockBack(knockBackForce);
            StartCoroutine(KnockBackCooldown());
        }
        
    }
    private IEnumerator KnockBackCooldown()
    {
        canKnockBack = false;
        yield return new WaitForSeconds(cooldownTime);
        canKnockBack = true;
    }
}
