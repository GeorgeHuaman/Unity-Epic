using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.Addons.SimpleKCC;
using UnityEngine;

public class PlayerFusion : NetworkBehaviour
{
    [SerializeField] private SimpleKCC kcc;
    [SerializeField] private float speed = 5;
    [SerializeField] private float jumpImpulse = 10f;

    [Networked] private NetworkButtons previousButtons { get; set; }
    public override void Spawned()
    {
        kcc.SetGravity(Physics.gravity.y * 2);
    }

    public override void FixedUpdateNetwork()
    {
        if(GetInput(out NetInput input))
        {
            Vector3 worldDirection = kcc.TransformRotation * new Vector3(input.direction.x, 0f, input.direction.y);
            float jump = 0;

            if (input.Buttons.WasPressed(previousButtons, InputButton.Jump) && kcc.IsGrounded)
                jump = jumpImpulse;

            kcc.Move(worldDirection.normalized * speed, jump);
            previousButtons = input.Buttons;
        }
    }
}
