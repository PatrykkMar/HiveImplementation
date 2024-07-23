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
    [SerializeField] private ClientStateMachine _clientStateMachine;


    public async Task InitializeMatchmakingServiceAsync()
    {
        _hubConnection = new HubConnectionBuilder()
        .WithUrl(_serverUrl, options =>
        {
            options.AccessTokenProvider = () => Task.FromResult(CurrentUser.Instance.Token);
        })
        .Build();

        _hubConnection.On<string, string, Trigger?>("ReceiveMessage", (player, message, trigger) =>
        {
            Debug.Log($"Player: {player}. Message from server: {message}. Has trigger: {trigger.HasValue}");
            _informationText.text = message;
            if(trigger.HasValue)
            {
                _clientStateMachine.Fire(trigger.Value);
            }
        });

        await _hubConnection.StartAsync();
        _informationText.text = "Connection started";
    }

    public async Task JoinQueueAsync()
    {
        await _hubConnection.InvokeAsync("JoinQueue");
    }
}
