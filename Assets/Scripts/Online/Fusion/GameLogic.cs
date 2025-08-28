using System.Collections;
using System.Collections.Generic;
using Fusion;
using Unity.Mathematics;
using UnityEngine;

public class GameLogic : NetworkBehaviour, IPlayerJoined, IPlayerLeft
{
    [SerializeField] private NetworkPrefabRef playerPrefab;
    [Networked, Capacity(40)] private NetworkDictionary<PlayerRef, PlayerFusion> Players => default;

    public void PlayerJoined(PlayerRef player)
    {
        if(HasStateAuthority)
        {
            NetworkObject playerObject = Runner.Spawn(playerPrefab, Vector3.up, quaternion.identity, player);
            Players.Add(player, playerObject.GetComponent<PlayerFusion>());
        }
    }

    public void PlayerLeft(PlayerRef player)
    {
        if(!HasStateAuthority) 
            return;
        if (Players.TryGet(player, out PlayerFusion playerFusion)) 
        {
            Players.Remove(player);
            Runner.Despawn(playerFusion.Object);
        }
    }
}
