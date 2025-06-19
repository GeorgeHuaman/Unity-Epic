using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class KnockbackOnImpact : MonoBehaviour
{

    public CharacterController controller;
    private bool canKnockBack = true;
    private bool canJump = true;
    private float cooldownTime = 0.2f;
    public float jumpForce = 10f;
    public float knockbackForce = 7f;
    public float gravity = -20f;
    public GameObject player;
    private Vector3 velocity;
    private bool isGrounded;
    void Update()
    {
        //isGrounded = controller.isGrounded;
        //if (isGrounded && velocity.y < 0)
        //    velocity.y = -2f;
        //velocity.y += gravity * Time.deltaTime;
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    Jump2();
        //}
        //controller.Move(velocity * Time.deltaTime);
    }

    public void Jump2()
    {
        //if (canJump && isGrounded)
        //    velocity.y = jumpForce*2;
    }
    public void KnockBack()
    {
        //if (canKnockBack)
        //{
        //    velocity.y = knockbackForce;
        //    StartCoroutine(KnockBackCooldown());
        //}
    }

    public void Jump()
    {
        //if (canJump && isGrounded)
        //{
        //    velocity.y = jumpForce;
        //    StartCoroutine(JumpCooldown());
        //}
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
