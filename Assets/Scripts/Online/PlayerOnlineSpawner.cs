using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOnlineSpawner : SimulationBehaviour, IPlayerJoined
{
    public GameObject playerOnlinePrefab;

    public void PlayerJoined(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
        {
            Runner.Spawn(playerOnlinePrefab, new Vector3(0,1,0),Quaternion.identity);
        }
    }
}
