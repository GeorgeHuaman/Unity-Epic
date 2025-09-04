using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.Addons.KCC;
using ReadyPlayerMe.Samples.QuickStart;
using UnityEngine;

public class PlayerFusion : NetworkBehaviour
{
    [SerializeField] private MeshRenderer[] modelParts;
    [SerializeField] public KCC kcc;
    [SerializeField] private Transform camTarget;
    [SerializeField] private float maxPitch = 85f;
    [SerializeField] private float sensibility = 0.15f;
    [SerializeField] private Vector3 jumpImpulse = new(0f,10f, 0f);
    [SerializeField] private float doubleJumpMultiplier = 0.75f;
    [SerializeField] private bool haveDoubleJump = true;

    public double score => Math.Round(transform.position.y, 1);
    public bool isReady = true;

    [Networked] public string name { get; private set; }
    [Networked] private NetworkButtons previousButtons { get; set; }
    private InputManager inputManager;
    private Vector2 baseLookRotation;
    private float verticalVelocity;

    [SerializeField] private AnimationController animationController;
    public override void Spawned()
    {
        if(HasInputAuthority)
        {
            foreach (MeshRenderer render in modelParts) 
                render.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;

            inputManager = Runner.GetComponent<InputManager>();
            inputManager.localplayer = this;
            name = PlayerPrefs.GetString("Photon.Menu.Username");
            RPC_PlayerName(name);
            CameraFollow.singleton.SetTarget(camTarget);
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetInput input))
        {
            CheckJump(input);
            kcc.AddLookRotation(input.lookDelta * sensibility, -maxPitch, maxPitch);

            SetInputDirection(input);
            previousButtons = input.Buttons;
            baseLookRotation = kcc.GetLookRotation();
        }

        if (kcc.FixedData.IsGrounded)
            haveDoubleJump = true;
    }

    [Rpc(RpcSources.InputAuthority,RpcTargets.StateAuthority | RpcTargets.InputAuthority)]
    public void RPC_SetReady()
    {
        isReady = true;
        if (HasInputAuthority)
            UIManager.singleton.DidSetReady();
    }
    public override void Render()
    {
        Vector2 lookToRender;

        if (kcc.IsPredictingLookRotation)
        {
            Vector2 predictedLookRotation = baseLookRotation + inputManager.AccumulatedMouseDelta * sensibility;
            kcc.SetLookRotation(predictedLookRotation);
            lookToRender = predictedLookRotation;
        }
        else
        {
            lookToRender = kcc.GetLookRotation();
        }

        Quaternion targetRot = Quaternion.Euler(lookToRender.x, 0f, 0f);
        camTarget.localRotation = Quaternion.Slerp(camTarget.localRotation, targetRot, Mathf.Clamp01(15f * Time.deltaTime));

        // sin suavizado:
        // camTarget.localRotation = targetRot;
    }

    public void Teleport(Vector3 position, Quaternion rotation)
    {
        kcc.SetPosition(position);
        kcc.SetLookRotation(rotation);
    }

    private void CheckJump(NetInput input)
    {
        if(input.Buttons.WasPressed(previousButtons, InputButton.Jump))
        {
            animationController.OnJump();

            if (kcc.FixedData.IsGrounded)
                kcc.Jump(jumpImpulse);
            else if (haveDoubleJump)
            {
                kcc.Jump(jumpImpulse * doubleJumpMultiplier);
                haveDoubleJump = false;
            }
        }
    }
    public void ResetCooldowns()
    {
        haveDoubleJump = true;
    }
    private void SetInputDirection(NetInput input)
    {
        Vector3 worldDirection = kcc.FixedData.TransformRotation * input.direction.X0Y();
        kcc.SetInputDirection(worldDirection);
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_PlayerName(string name)
    {
        this.name = name;
    }
}
