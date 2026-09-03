using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayersJoin : NetworkBehaviour
{
    public Button hostButton, clientButton;
    public bool IsConnected => NetworkManager!.IsClient || NetworkManager!.IsServer;
    public int PlayerCount => playerCount.Value;
    readonly NetworkVariable<int> playerCount = new();
    
    /*public bool IsReady => isReady.Value;
    readonly NetworkVariable<bool> isReady = new(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);*/
    
    public PaddleSide paddleSide;

    // public SessionManager sessionManager;
    public GameManager gameManager;
    // public Paddle

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
    
    private void Awake()
    {
        Button leftButton = hostButton.GetComponent<Button>(); 
        Button rightButton = clientButton.GetComponent<Button>();
        Debug.Log("Player Count: " + playerCount.Value);
        
        // host is assigned the left paddle
        // increment the player count by one
        // After the host button is clicked then the 
        // host button disappears and the left paddle appears
        leftButton.onClick.AddListener(() => 
        {
            hostButton.gameObject.SetActive(false);
            Debug.Log("Host Button clicked");
            paddleSide = PaddleSide.Left;
            playerCount.Value++;
            Debug.Log("Player Count: " + playerCount.Value);
        });

        // client is assigned the right paddle
        // increment the player count by one
        // after the client button is clicked then 
        // the client button disappears and the right paddle appears
        // and the game starts too 
        rightButton.onClick.AddListener(() =>
        {
            clientButton.gameObject.SetActive(false);
            Debug.Log("Client Button clicked");
            paddleSide = PaddleSide.Right;
            playerCount.Value++;
            Debug.Log("Player Count: " + playerCount.Value);

            if (playerCount.Value == 2)
            {
                Debug.Log("Start Game!");
                // gameManager.StartGame();
            }
        });
        // Debug.Log("Hi");
    }

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
    }

    private void UpdatePlayerCount()
    {
        playerCount.Value = NetworkManager.ConnectedClientsIds.Count;
        Debug.Log("Player Value: " + playerCount.Value);
    }
}
