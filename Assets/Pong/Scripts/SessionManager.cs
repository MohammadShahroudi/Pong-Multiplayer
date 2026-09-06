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
    public PaddleSide paddleSide;
    
    // This does not already exist in the scene, you need to add it and reference it

    [Header("Multiplayer UI")]
    [SerializeField] Canvas sessionUI; 
    [SerializeField] Button startHostButton;
    [SerializeField] Button startClientButton;

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

    private void Awake()
    {
        // networkManager = FindObjectOfType<NetworkManager>();
        // host is assigned the left paddle
        // increment the player count by one
        // After the host button is clicked then the 
        // host button disappears and the left paddle appears
        startHostButton.onClick.AddListener(() =>
        {
            startHostButton.gameObject.SetActive(false);
            NetworkManager.StartHost();
            Debug.Log("Host Button clicked");
            paddleSide = PaddleSide.Left;

            // client is assigned the right paddle
            // increment the player count by one
            // after the client button is clicked then 
            // the client button disappears and the right paddle appears
            // and the game starts too 
        });
        
        startClientButton.onClick.AddListener(() =>
        {
            startClientButton.gameObject.SetActive(false);
            NetworkManager.StartClient();
            Debug.Log("Client Button clicked");
            paddleSide = PaddleSide.Right;
        });
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        
        name = $"Player {NetworkObject.OwnerClientId}";
        
        Debug.Log(
            $"[PlayerIdentity] {name} spawned | ownerClientId={NetworkObject.OwnerClientId} | " +
            $"isOwner={IsOwner} | isServer={IsServer} | isClient={IsClient} | isHost={IsHost}");
        
        if (!IsServer) return;
        // if (!IsOwner) return;
        
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
    // public void Disconnect() => NetworkManager!.Shutdown();

    private void HandleConnectionEvent(NetworkManager networkManager, ConnectionEventData eventData)
    {
        Debug.Assert(IsServer);
        UpdatePlayerCount();
        Debug.Log("Player Value: " + playerCount.Value);
        
        if (playerCount.Value == 2)
        {
            Debug.Log("Both players have been connected!");
            gameManager.UpdateScore();
            gameManager.StartGame();
        }
    }

    private void UpdatePlayerCount()
    {
        playerCount.Value = NetworkManager.ConnectedClientsIds.Count;
    }
}