using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;


public enum InputButton
{
    Jump,
}

public struct NetInput: INetworkInput
{
    public NetworkButtons Buttons;
    public Vector2 direction;
    public Vector2 lookDelta;
}