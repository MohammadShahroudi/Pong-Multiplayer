using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

/*
 * SessionManager is intentionally light in the starter project.
 * Local Pong starts immediately. The GameManager, NetworkManager, and button
 * references mark where you will start a Host or Client session.
 */

public class SessionManager : NetworkBehaviour
{
    [Header("Multiplayer")]
    [SerializeField] GameManager gameManager;
    
    // This does not already exist in the scene, you need to add it and reference it
    [SerializeField] NetworkManager networkManager;

    [Header("Multiplayer UI")]
    [SerializeField] Canvas sessionUI;

    public bool IsConnected => NetworkManager!.IsClient || NetworkManager!.IsServer;
    public int PlayerCount => playerCount.Value;

    public string LocalRole
    {
        get
        {
            if (NetworkManager!.IsHost) return "Host";
            if (NetworkManager!.IsServer) return "Server";
            if (NetworkManager!.IsClient) return "Client";
            
            return "Disconnected";
        }
    }
    
    readonly NetworkVariable<int> playerCount = new();

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsServer) return;
        if (!IsOwner) return;
        
        UpdatePlayerCount();
        NetworkManager.OnConnectionEvent += HandleConnectionEvent;
    }
    
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        if (!NetworkManager!.IsServer) return;
        
        NetworkManager.OnConnectionEvent -= HandleConnectionEvent;
        playerCount.Value = 0;
    }
    
    public void StartHost() => NetworkManager!.StartHost();
    public void StartClient() => NetworkManager!.StartClient();

    private void HandleConnectionEvent(NetworkManager networkManager, ConnectionEventData eventData)
    {
        Debug.Assert(IsServer);
        UpdatePlayerCount();
        Debug.Log("Player Value: " + playerCount.Value);
    }

    private void UpdatePlayerCount()
    {
        playerCount.Value = NetworkManager.ConnectedClientsIds.Count;
    }
}