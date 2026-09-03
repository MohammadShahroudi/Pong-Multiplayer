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
    public GameManager gameManager;
    

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
        
        // host is assigned the left paddle
        // After the host button is clicked then the 
        // host button disappears and the left paddle appears
        leftButton.onClick.AddListener(() => 
        {
            hostButton.gameObject.SetActive(false);
            Debug.Log("Host Button clicked");
        });

        // client is assigned the right paddle
        // after the client button is clicked then 
        // the client button disappears and the right paddle appears
        // and the game starts too 
        rightButton.onClick.AddListener(() =>
        {
            clientButton.gameObject.SetActive(false);
            Debug.Log("Client Button clicked");
            gameManager.StartGame();
        });
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsServer) return;

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

    private void HandleConnectionEvent(NetworkManager networkManager, ConnectionEventData eventData)
    {
        Debug.Assert(IsServer);
        UpdatePlayerCount();
    }

    private void UpdatePlayerCount()
    {
        playerCount.Value = NetworkManager.ConnectedClientsIds.Count;
    }
}
