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

    [Header("Multiplayer UI")]
    [SerializeField] Canvas sessionUI; 
    [SerializeField] Button startHostButton;
    [SerializeField] Button startClientButton;
    [SerializeField] Paddle leftPaddle;
    [SerializeField] Paddle rightPaddle;

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
    // private readonly NetworkVariable<PaddleSide> paddleSide = new();
    // public bool HasSpawned => IsSpawned;
    // public int SpawnSlot => spawnSlot.Value;

    private void Awake()
    {
        // PaddleSide playerSide;
        // networkManager = FindObjectOfType<NetworkManager>();
        // host is assigned the left paddle
        // increment the player count by one
        // After the host button is clicked then the 
        // host button disappears and the left paddle appears
        startHostButton.onClick.AddListener(() =>
        {
            startHostButton.gameObject.SetActive(false);
            NetworkManager.StartHost();
            AssignSpawnSlot();
            Debug.Log("Host Button clicked");
        });
        
        // client is assigned the right paddle
        // increment the player count by one
        // after the client button is clicked then 
        // the client button disappears and the right paddle appears
        // and the game starts too 
        startClientButton.onClick.AddListener(() =>
        {
            // startClientButton.gameObject.SetActive(false);
            NetworkManager.StartClient();
            AssignSpawnSlot();
            Debug.Log("Client Button clicked");
        });
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        // if (!IsServer) return;
        
        if (!IsOwner) return;
        
        name = $"Player {NetworkObject.OwnerClientId}";
        
        UpdatePlayerCount();
        // AssignSpawnSlot();
        NetworkManager.OnConnectionEvent += HandleConnectionEvent;
    }
    
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        
        // Debug.Log($"[PlayerIdentity] {name} despawned");
        
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
        // AssignSpawnSlot();
        Debug.Log("Player Value: " + playerCount.Value);
        Debug.Log(
            $"[PlayerIdentity] {name} spawned | ownerClientId={NetworkObject.OwnerClientId} | " +
            $"isOwner={IsOwner} | isServer={IsServer} | isClient={IsClient} | isHost={IsHost}");
        
        if (playerCount.Value == 2)
        {
            Debug.Log("Both players have been connected!");
            // AssignSpawnSlot();
            HideButtonRpc();
            gameManager.StartGame();
            gameManager.UpdateScore();
        }
    }

    [Rpc(SendTo.Everyone)]
    private void HideButtonRpc()
    {
        startHostButton.gameObject.SetActive(false);
        startClientButton.gameObject.SetActive(false);
    }
    
    private void UpdatePlayerCount()
    {
        playerCount.Value = NetworkManager.ConnectedClientsIds.Count;
    }

    void AssignSpawnSlot()
    {
        PaddleSide playerSide;
        if (IsHost)
        {
            playerSide = PaddleSide.Left;
            leftPaddle.gameObject.SetActive(true);
            Debug.Log(leftPaddle.ToString());
            // Debug.Log("Player Side: " + playerSide.ToString());
        }
        else 
        {
            playerSide = PaddleSide.Right;
            rightPaddle.gameObject.SetActive(true);
            Debug.Log(rightPaddle.ToString());
            // Debug.Log("Player Side: " + playerSide.ToString());
        }
    }
}