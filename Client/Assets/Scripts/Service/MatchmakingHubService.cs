using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using UnityEngine;

public class MatchmakingHubService : MonoBehaviour
{
    private HubConnection _hubConnection;

    [SerializeField] private string _serverUrl = "ws://localhost:7200/matchmakinghub";


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

        await _hubConnection.StartAsync();
        Debug.Log("Connection started");
    }

    public async Task JoinQueueAsync()
    {
        await _hubConnection.InvokeAsync("JoinQueue");
    }
}
