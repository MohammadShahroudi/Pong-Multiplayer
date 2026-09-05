using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayersJoin : NetworkBehaviour
{
    [SerializeField] SessionManager sessionManager;
    
    [SerializeField] Button startHostButton;
    [SerializeField] Button startClientButton;
    
    public PaddleSide paddleSide;
    
    private void Awake()
    {
        if (sessionManager.IsConnected)
        {
            Debug.Log("Session Manager is connected");
        }
        else
        {
            // host is assigned the left paddle
            // increment the player count by one
            // After the host button is clicked then the 
            // host button disappears and the left paddle appears
            startHostButton.onClick.AddListener(() =>
            {
                startHostButton.gameObject.SetActive(false);
                sessionManager.StartHost();
                Debug.Log("Host Button clicked");
                paddleSide = PaddleSide.Left;
            });
        
            // client is assigned the right paddle
            // increment the player count by one
            // after the client button is clicked then 
            // the client button disappears and the right paddle appears
            // and the game starts too 
            startClientButton.onClick.AddListener(() =>
            {
                startClientButton.gameObject.SetActive(false);
                sessionManager.StartClient();
                Debug.Log("Client Button clicked");
                paddleSide = PaddleSide.Right;
            });
        }
    }
}
