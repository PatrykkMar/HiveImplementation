using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.AspNetCore.SignalR.Client;
using TMPro;

public class GameManager : MonoBehaviour
{
    public Button joinButton;
    public Button sendButton;
    public Text informationText;
    private HubConnection connection;
    [SerializeField] private MatchmakingHubService _service;
    [SerializeField] private TokenService _token;

    async void Awake()
    {
        joinButton.onClick.AddListener(JoinQueue);
        sendButton.onClick.AddListener(SendMessageToServer);
        StartCoroutine(_token.GetToken());
        await _service.InitializeAsync();
    }

    async void JoinQueue()
    {
        await _service.JoinQueueAsync();
    }

    async void SendMessageToServer()
    {
        if (connection == null || connection.State != HubConnectionState.Connected)
        {
            DisplayMessage("Not connected to the server.");
            return;
        }

        string user = "test";
        string message = "test";

        try
        {
            await connection.InvokeAsync("SendMessage", user, message);
        }
        catch (Exception ex)
        {
            DisplayMessage($"Send failed: {ex.Message}");
        }
    }

    
    void DisplayMessage(string message)
    {
        informationText.text = message;
        Debug.Log(message);
    }
}