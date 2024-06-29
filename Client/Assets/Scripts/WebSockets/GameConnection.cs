using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.AspNetCore.SignalR.Client;
using TMPro;

public class GameConnection : MonoBehaviour
{
    public InputField nickInputField;
    public Button joinButton;
    public Button sendButton;
    public Text informationText;
    private HubConnection connection;

    private string Adres = "http://localhost:7200";

    void Start()
    {
        joinButton.onClick.AddListener(ConnectToServer);
        sendButton.onClick.AddListener(SendMessageToServer);
    }

    async void ConnectToServer()
    {
        string serverAddress = Adres;

        connection = new HubConnectionBuilder()
            .WithUrl(serverAddress + "/gamehub")
            .Build();

        connection.On<string, string>("ReceiveMessage", (user, message) =>
        {
            DisplayMessage($"{user}: {message}");
        });

        try
        {
            await connection.StartAsync();
            DisplayMessage("Connected to the server.");
        }
        catch (Exception ex)
        {
            DisplayMessage($"Connection failed: {ex.Message}");
        }
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
    }
}