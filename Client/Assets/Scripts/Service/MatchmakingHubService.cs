using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using UnityEngine;
using UnityEngine.UI;

public class MatchmakingHubService : MonoBehaviour
{
    private HubConnection _hubConnection;

    [SerializeField] private string _serverUrl = "ws://localhost:7200/matchmakinghub";
    [SerializeField] private Text _informationText;


    public async Task InitializeAsync()
    {
        Debug.Log(_serverUrl);
        Debug.Log(CurrentUser.Instance.Token);
        _hubConnection = new HubConnectionBuilder()
        .WithUrl(_serverUrl, options =>
        {
            options.AccessTokenProvider = () => Task.FromResult(CurrentUser.Instance.Token);
        })
        .Build();

        _hubConnection.On<string, string>("ReceiveMessage", (player, message) =>
        {
            Debug.Log($"Player: {player}. Message from server: " + message);
            _informationText.text = message;
        });

        await _hubConnection.StartAsync();
        _informationText.text = "Connection started";
    }

    public async Task JoinQueueAsync()
    {
        await _hubConnection.InvokeAsync("JoinQueue");
    }
}
