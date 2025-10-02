using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class TPCentralOnline : MonoBehaviour
{
    public GameObject playerOnline;
    public Transform temas, centroIA, centro, cine;
    public GameObject panelMapa;
    
    private NetworkRunner runner;
    
    void Start()
    {
        // Buscar el NetworkRunner en la escena
        runner = FindObjectOfType<NetworkRunner>();
    }
    
    // Método para encontrar al jugador local
    private PlayerFusion GetLocalPlayer()
    {
        if (runner == null)
        {
            runner = FindObjectOfType<NetworkRunner>();
            Debug.Log("[TPCentralOnline] NetworkRunner encontrado: " + (runner != null ? "SÍ" : "NO"));
        }
        
        if (runner != null)
        {
            // Buscar todos los PlayerFusion en la escena
            PlayerFusion[] allPlayers = FindObjectsOfType<PlayerFusion>();
            Debug.Log($"[TPCentralOnline] Jugadores encontrados en escena: {allPlayers.Length}");
            
            foreach (PlayerFusion player in allPlayers)
            {
                Debug.Log($"[TPCentralOnline] Jugador: {player.name} - HasInputAuthority: {player.HasInputAuthority}");
                if (player.HasInputAuthority)
                {
                    Debug.Log($"[TPCentralOnline] Jugador local encontrado: {player.name}");
                    return player;
                }
            }
        }
        
        // Fallback: usar el playerOnline asignado manualmente
        if (playerOnline != null)
        {
            Debug.Log("[TPCentralOnline] Usando playerOnline asignado manualmente");
            return playerOnline.GetComponent<PlayerFusion>();
        }
        
        Debug.LogWarning("[TPCentralOnline] No se encontró jugador local!");
        return null;
    }
    public void TpTemas()
    {
        Debug.Log("[TPCentralOnline] TpTemas() llamado");
        PlayerFusion localPlayer = GetLocalPlayer();
        if (localPlayer != null && temas != null)
        {
            Debug.Log($"[TPCentralOnline] Teleportando a TEMAS - Posición: {temas.position}");
            localPlayer.RequestTeleport(temas.position, temas.rotation);
            StartCoroutine(WaitAndOpenMap());
        }
        else
        {
            Debug.LogWarning("[TPCentralOnline] TpTemas falló - Jugador: " + (localPlayer != null ? "OK" : "NULL") + 
                           " | Temas: " + (temas != null ? "OK" : "NULL"));
        }
    }

    public void TpCentroIA()
    {
        Debug.Log("[TPCentralOnline] TpCentroIA() llamado");
        PlayerFusion localPlayer = GetLocalPlayer();
        if (localPlayer != null && centroIA != null)
        {
            Debug.Log($"[TPCentralOnline] Teleportando a CENTRO IA - Posición: {centroIA.position}");
            localPlayer.RequestTeleport(centroIA.position, centroIA.rotation);
            StartCoroutine(WaitAndOpenMap());
        }
        else
        {
            Debug.LogWarning("[TPCentralOnline] TpCentroIA falló - Jugador: " + (localPlayer != null ? "OK" : "NULL") + 
                           " | CentroIA: " + (centroIA != null ? "OK" : "NULL"));
        }
    }

    public void TpCentro()
    {
        Debug.Log("[TPCentralOnline] TpCentro() llamado");
        PlayerFusion localPlayer = GetLocalPlayer();
        if (localPlayer != null && centro != null)
        {
            Debug.Log($"[TPCentralOnline] Teleportando a CENTRO - Posición: {centro.position}");
            localPlayer.RequestTeleport(centro.position, centro.rotation);
            StartCoroutine(WaitAndOpenMap());
        }
        else
        {
            Debug.LogWarning("[TPCentralOnline] TpCentro falló - Jugador: " + (localPlayer != null ? "OK" : "NULL") + 
                           " | Centro: " + (centro != null ? "OK" : "NULL"));
        }
    }
    
    public void TpCine()
    {
        Debug.Log("[TPCentralOnline] TpCine() llamado");
        PlayerFusion localPlayer = GetLocalPlayer();
        if (localPlayer != null && cine != null)
        {
            Debug.Log($"[TPCentralOnline] Teleportando a CINE - Posición: {cine.position}");
            localPlayer.RequestTeleport(cine.position, cine.rotation);
            StartCoroutine(WaitAndOpenMap());
        }
        else
        {
            Debug.LogWarning("[TPCentralOnline] TpCine falló - Jugador: " + (localPlayer != null ? "OK" : "NULL") + 
                           " | Cine: " + (cine != null ? "OK" : "NULL"));
        }
    }
    public void ButtonMapa()
    {
        GameManager gm = GameManager.Instance;
        gm.SetIsCanvasOpen(!panelMapa.activeSelf);
        panelMapa.SetActive(gm.IsCanvasOpen());
    }

    public void Teleport(Transform targetTransform)
    {
        Debug.Log($"[TPCentralOnline] Teleport() genérico llamado - Target: {(targetTransform != null ? targetTransform.name : "NULL")}");
        PlayerFusion localPlayer = GetLocalPlayer();
        if (localPlayer != null && targetTransform != null)
        {
            Debug.Log($"[TPCentralOnline] Teleportando a {targetTransform.name} - Posición: {targetTransform.position}");
            localPlayer.RequestTeleport(targetTransform.position, targetTransform.rotation);
        }
        else
        {
            Debug.LogWarning("[TPCentralOnline] Teleport genérico falló - Jugador: " + (localPlayer != null ? "OK" : "NULL") + 
                           " | Target: " + (targetTransform != null ? "OK" : "NULL"));
        }
    }
    private IEnumerator WaitAndOpenMap()
    {
        yield return new WaitForSeconds(0.2f);
        ButtonMapa();
    }
}

