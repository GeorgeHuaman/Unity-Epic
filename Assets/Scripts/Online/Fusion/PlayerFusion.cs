using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.Addons.KCC;
using UnityEngine;

public class PlayerFusion : NetworkBehaviour
{
    [SerializeField] private MeshRenderer[] modelParts;
    [SerializeField] private KCC kcc;
    [SerializeField] private Transform camTarget;
    [SerializeField] private float sensibility = 0.15f;
    [SerializeField] private Vector3 jumpImpulse = new(0f,10f, 0f);

    public double score => Math.Round(transform.position.y, 1);
    public bool isReady = true;
    private float maxPitch;

    [Networked] public string name { get; private set; }
    [Networked] private NetworkButtons previousButtons { get; set; }
    private InputManager inputManager;
    private Vector2 baseLookRotation;
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
            kcc.Settings.ForcePredictedLookRotation = true;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetInput input))
        {
            CheckJump(input);
            Vector2 lookDelta = input.lookDelta * sensibility;
            kcc.AddLookRotation(input.lookDelta * sensibility, -maxPitch, maxPitch);
            UpdateCamera();

            SetInputDirection(input);
            previousButtons = input.Buttons;
            baseLookRotation = kcc.GetLookRotation();
        }
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
        if (kcc.Settings.ForcePredictedLookRotation)
        {
            Vector2 predictedLookRotation = baseLookRotation + inputManager.AccumulatedMouseDelta * sensibility;
            kcc.SetLookRotation(predictedLookRotation);
        }
    }
    private void UpdateCamera()
    {
        camTarget.localRotation = Quaternion.Euler(kcc.GetLookRotation().x, 0f, 0f);
    }

    public void Teleport(Vector3 position, Quaternion rotation)
    {
        kcc.SetPosition(position);
        kcc.SetLookRotation(rotation);
    }

    private void CheckJump(NetInput input)
    {
        if(input.Buttons.WasPressed(previousButtons, InputButton.Jump) && kcc.FixedData.IsGrounded)
        {
            kcc.Jump(jumpImpulse);
        }
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
