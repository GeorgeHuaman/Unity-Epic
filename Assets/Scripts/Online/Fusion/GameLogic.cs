using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using Unity.Mathematics;
using UnityEngine;

public enum GameState
{
    Wait,
    Playing,
    Win,
}

public class GameLogic : NetworkBehaviour, IPlayerJoined, IPlayerLeft
{
    [SerializeField] private NetworkPrefabRef playerPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform SpawnPointPivot;

    [SerializeField] private PlayerFusion winner;
    [SerializeField, OnChangedRender(nameof(GameStateChange))] private GameState state { get; set; }


    [Networked, Capacity(40)] private NetworkDictionary<PlayerRef, PlayerFusion> Players => default;


    public override void Spawned()
    {
        winner = null;
        state = GameState.Wait;
        UIManager.singleton.SetWait(state, winner);
        Runner.SetIsSimulated(Object, true);
    }


    private void UnReadyAll()
    {
        foreach(KeyValuePair<PlayerRef,PlayerFusion> player in Players)
            player.Value.isReady = false;
    }

    public override void FixedUpdateNetwork()
    {
        if (Players.Count <= 1)
            return;
        if (Runner.IsServer && state == GameState.Wait)
        {
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
                winner = null;
                state = GameState.Playing;
                PreparePlayers();
                //Variable de estado para inicio
            }
            if(state == GameState.Playing && !Runner.IsResimulation)
                UIManager.singleton.UpdateLeaderBoard(Players.OrderByDescending(p =>p.Value.score).ToArray());
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

    private void GameStateChange()
    {
        UIManager.singleton.SetWait(state,winner );
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
