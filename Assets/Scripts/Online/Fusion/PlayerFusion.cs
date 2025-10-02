using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    
    // Flag para controlar el teleport
    private bool isTeleporting = false;
    private Vector3 teleportPosition;
    private Quaternion teleportRotation;
    // teleportSpeed ahora se usa desde teleportSpeedMultiplier
    private InputManager inputManager;
    private Vector2 baseLookRotation;
    private float verticalVelocity;

    // --- Referencias/cache ---
    Rigidbody rb;
    Collider[] cachedColliders;
    bool[] cachedColliderStates;
    bool collidersDisabledForTeleport = false;
    // --- Ajustables ---
    [SerializeField] private float teleportSpeedMultiplier = 200f; // Velocidad del teleport (ajustable desde inspector)
    public float stopDistance = 4f;
    [SerializeField] private AnimationController animationController;
    public override void Spawned()
    {
        if(HasInputAuthority)
        {
            rb = GetComponent<Rigidbody>();

            foreach (MeshRenderer render in modelParts) 
                render.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;

            inputManager = Runner.GetComponent<InputManager>();
            inputManager.localplayer = this;
            name = PlayerPrefs.GetString("Photon.Menu.Username");
            RPC_PlayerName(name);
            cachedColliders = GetComponentsInChildren<Collider>(true);
            cachedColliderStates = cachedColliders.Select(c => c.enabled).ToArray();
            CameraFollow.singleton.SetTarget(camTarget);
            ManagerInteractuable._singleton.player = this.gameObject;
            ManagerInteractuable._singleton.DistribuidEmptyPlayer();
        }
    }
    public override void FixedUpdateNetwork()
    {
        if (isTeleporting)
        {
            
            Vector3 currentPosition = transform.position;
            Vector3 toTarget = teleportPosition - currentPosition;
            float distance = toTarget.magnitude;
            Vector3 direction = toTarget.normalized;


            if (distance < stopDistance)
            {
                UIManager.singleton.ScreenLoadActive(false);
                ComponentPlayerOnline.singleton.ModelIsActive(true);
                // --- Teleport finalizado ---
                if (kcc != null)
                {
                    kcc.SetInputDirection(Vector3.zero);
                }
                else
                {
                    transform.position = teleportPosition;
                    transform.rotation = teleportRotation;
                }

                isTeleporting = false;
                return;
            }

            // --- Movimiento usando únicamente el inputDirection del KCC ---
            if (kcc != null)
            {
                UIManager.singleton.ScreenLoadActive(true);
                ComponentPlayerOnline.singleton.ModelIsActive(false);
                // El KCC normalmente espera dirección normalizada (0..1)
                Vector3 inputDir = new Vector3(direction.x, 0f, direction.z).normalized;
                kcc.SetInputDirection(inputDir * teleportSpeedMultiplier);
            }

            // Rotar mirando al destino
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Runner.DeltaTime);
            }

            return;
        }

        // --- flujo normal ---
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
    //[Rpc(RpcSources.InputAuthority,RpcTargets.StateAuthority | RpcTargets.InputAuthority)]
    //public void RPC_SetReady()
    //{
    //    isReady = true;
    //    if (HasInputAuthority)
    //        UIManager.singleton.DidSetReady();
    //}
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
        // Método local para teleport (sin red)
        PerformTeleport(position, rotation);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_Teleport(Vector3 position, Quaternion rotation)
    {
        // Debug: Mostrar posición antes del teleport
        Vector3 oldPosition = transform.position;
        Debug.Log($"[TELEPORT] Jugador {name} - Posición ANTES: {oldPosition}");
        Debug.Log($"[TELEPORT] Jugador {name} - Posición DESTINO: {position}");
        
        // Ejecutar el teleport
        PerformTeleport(position, rotation);
        
        // Debug: Verificar que la posición cambió
        Debug.Log($"[TELEPORT] Jugador {name} - Posición DESPUÉS: {transform.position}");
        Debug.Log($"[TELEPORT] Jugador {name} - ¿Cambió posición? {(Vector3.Distance(oldPosition, transform.position) > 0.1f ? "SÍ" : "NO")}");
    }

    private void PerformTeleport(Vector3 position, Quaternion rotation)
    {
        // Activar el flag de teleport y guardar la posición destino
        isTeleporting = true;
        teleportPosition = position;
        teleportRotation = rotation;
        
        Debug.Log($"[TELEPORT] Teleport iniciado para {name} - Posición destino: {position} - Velocidad: {teleportSpeedMultiplier} (MUY ALTA)");
    }

    // Método público para llamar el teleport desde otros scripts
    public void RequestTeleport(Vector3 position, Quaternion rotation)
    {
        Debug.Log($"[TELEPORT] {name} solicita teleport a posición: {position}");
        // Cualquier jugador puede solicitar teleport
        RPC_Teleport(position, rotation);
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
