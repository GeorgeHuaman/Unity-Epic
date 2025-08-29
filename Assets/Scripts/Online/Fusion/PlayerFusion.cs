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
    private float pitch;        // acumulador real del pitch
    private float smoothPitch;  // pitch suavizado
    [SerializeField] private float cameraSmooth = 10f; // velocidad de interpolación
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
        pitch = 0f;
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetInput input))
        {
            Vector2 lookDelta = input.lookDelta * sensibility;

            // --- YAW ---
            kcc.AddLookRotation(new Vector2(0, lookDelta.y));

            // --- PITCH ---
            pitch += lookDelta.x;
            pitch = Mathf.Clamp(pitch, -80f, 80f);

            // --- MOVIMIENTO ---
            Vector3 worldDirection = kcc.TransformRotation * new Vector3(input.direction.x, 0f, input.direction.y);
            float jump = 0;

            if (input.Buttons.WasPressed(previousButtons, InputButton.Jump) && kcc.IsGrounded)
                jump = jumpImpulse;

            kcc.Move(worldDirection.normalized * speed, jump);
            previousButtons = input.Buttons;
        }
    }

    public override void Render()
    {
        UpdateCamera();
    }
    private void UpdateCamera()
    {
        smoothPitch = Mathf.Lerp(smoothPitch, pitch, Time.deltaTime * cameraSmooth);

        camTarget.localRotation = Quaternion.Euler(smoothPitch, 0, 0);
    }

    public void Teleport(Vector3 position, Quaternion rotation)
    {
        kcc.SetPosition(position);
        kcc.SetLookRotation(rotation);
    }
}
