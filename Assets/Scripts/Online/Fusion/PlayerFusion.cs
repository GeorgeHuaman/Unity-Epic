using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.Addons.SimpleKCC;
using UnityEngine;

public class PlayerFusion : NetworkBehaviour
{
    [SerializeField] private MeshRenderer[] modelParts;
    [SerializeField] private SimpleKCC kcc;
    [SerializeField] private Transform camTarget;
    [SerializeField] private float sensibility = 0.15f;
    [SerializeField] private float speed = 5;
    [SerializeField] private float jumpImpulse = 10f;

    public bool isReady = true;

    [Networked] private NetworkButtons previousButtons { get; set; }
    public override void Spawned()
    {
        kcc.SetGravity(Physics.gravity.y * 2);

        if(HasInputAuthority)
        {
            foreach (MeshRenderer render in modelParts) 
                render.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;

            CameraFollow.singleton.SetTarget(camTarget);
        }
    }

    public override void FixedUpdateNetwork()
    {
        if(GetInput(out NetInput input))
        {
            kcc.AddLookRotation(input.lookDelta * sensibility);
            UpdateCamera();
            Vector3 worldDirection = kcc.TransformRotation * new Vector3(input.direction.x, 0f, input.direction.y);
            float jump = 0;

            if (input.Buttons.WasPressed(previousButtons, InputButton.Jump) && kcc.IsGrounded)
                jump = jumpImpulse;

            kcc.Move(worldDirection.normalized * speed, jump);
            previousButtons = input.Buttons;
        }
    }

    //public override void Render()
    //{
    //    UpdateCamera();
    //}
    private void UpdateCamera()
    {
        camTarget.localRotation = Quaternion.Euler(kcc.GetLookRotation().x, 0, 0);
    }

    public void Teleport(Vector3 position, Quaternion rotation)
    {
        kcc.SetPosition(position);
        kcc.SetLookRotation(rotation);
    }
}
