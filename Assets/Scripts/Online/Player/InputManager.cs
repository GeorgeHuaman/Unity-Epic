using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fusion;
using Fusion.Menu;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : SimulationBehaviour, IBeforeUpdate, INetworkRunnerCallbacks
{
    private NetInput accumulatedInput;
    private bool resetInput;
    public void BeforeUpdate()
    {
        if(resetInput)
        {
            resetInput = false;
            accumulatedInput = default;
        }

        Keyboard keyboard = Keyboard.current;
        if (keyboard != null && (keyboard.enterKey.wasPressedThisFrame || keyboard.numpadEnterKey.wasPressedThisFrame || keyboard.escapeKey.wasPressedThisFrame))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None ;
                Cursor.visible = true ;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
        if (Cursor.lockState != CursorLockMode.Locked)
            return;

        NetworkButtons buttons = default;

        Mouse mouse = Mouse.current;
        if (mouse != null)
        {
            Vector2 mouseDelta = mouse.delta.ReadValue();
            Vector2 lookRotationDelta = new(-mouseDelta.y, mouseDelta.x);
            accumulatedInput.lookDelta += lookRotationDelta;
        }

        if (keyboard != null)
        {
            Vector2 moveDirection = Vector2.zero;

            if(keyboard.wKey.isPressed)
                moveDirection += Vector2.up;
            if (keyboard.sKey.isPressed)
                moveDirection += Vector2.down;
            if (keyboard.aKey.isPressed)
                moveDirection += Vector2.left;
            if (keyboard.dKey.isPressed)
                moveDirection += Vector2.right;

            accumulatedInput.direction += moveDirection;

            buttons.Set(InputButton.Jump, keyboard.spaceKey.isPressed);
        }

        accumulatedInput.Buttons = new NetworkButtons(accumulatedInput.Buttons.Bits | buttons.Bits);
    }

    public void OnConnectedToServer(NetworkRunner runner) { }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }


    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) {}

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken){}

    public void OnInput(NetworkRunner runner, NetworkInput input) 
    { 
        accumulatedInput.direction.Normalize();
        input.Set(accumulatedInput);
        resetInput = true;

        accumulatedInput.lookDelta = default;
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input){}

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (player == runner.LocalPlayer)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data){}

    public void OnSceneLoadDone(NetworkRunner runner){}

    public void OnSceneLoadStart(NetworkRunner runner){}

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList){}

    public async void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (shutdownReason == ShutdownReason.DisconnectedByPluginLogic)
        {
            await FindFirstObjectByType<FusionMenuConnectionBehaviour>(FindObjectsInactive.Include).DisconnectAsync(ConnectFailReason.Disconnect);
            FindFirstObjectByType<FusionMenuUIGameplay>(FindObjectsInactive.Include).Controller.Show<FusionMenuUIMain>();
        }
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message){}
}
