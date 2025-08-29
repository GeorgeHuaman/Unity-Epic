using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Unity.Mathematics;
using UnityEngine;

public class GameLogic : NetworkBehaviour, IPlayerJoined, IPlayerLeft
{
    [SerializeField] private NetworkPrefabRef playerPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform SpawnPointPivot;

    [Networked, Capacity(40)] private NetworkDictionary<PlayerRef, PlayerFusion> Players => default;


    private void UnReadyAll()
    {
        foreach(KeyValuePair<PlayerRef,PlayerFusion> player in Players)
            player.Value.isReady = false;
    }

    public override void FixedUpdateNetwork()
    {
        if (Players.Count <= 1)
            return;

        bool areAllReady = true;

        foreach (KeyValuePair<PlayerRef, PlayerFusion> player in Players)
        {
            if (!player.Value.isReady)
            {
                areAllReady = false;
                break;
            }
        }

        if (areAllReady)
        {
            PreparePlayers();
            //Variable de estado para inicio
        }

    }

    private void PreparePlayers()
    {
        float spaceAngle = 360 / Players.Count;
        SpawnPointPivot.rotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0, 360f), 0);
        foreach (KeyValuePair<PlayerRef, PlayerFusion> player in Players)
        {
            GetNextSpawnPoint(spaceAngle, out Vector3 position, out Quaternion rotation);
            player.Value.Teleport(position, rotation);
        }
    }
    private void GetNextSpawnPoint(float spaceAngle,out Vector3 position, out Quaternion rotation)
    {
        position = spawnPoint.position;
        rotation = spawnPoint.rotation;
        SpawnPointPivot.Rotate(0f, spaceAngle, 0f);
    }

    public void PlayerJoined(PlayerRef player)
    {
        if(HasStateAuthority)
        {
            GetNextSpawnPoint(90f, out Vector3 position, out Quaternion rotation);
            NetworkObject playerObject = Runner.Spawn(playerPrefab, position, rotation, player);
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
