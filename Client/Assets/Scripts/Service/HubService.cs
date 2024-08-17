using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using UnityEngine;
using UnityEngine.UI;

public class HubService : MonoBehaviour
{
    public const string ChosenVertexKey = "ChosenVertexKey";

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

        _hubConnection.On<string, string, Trigger?, PlayerViewDTO>("ReceiveMessage", (player, message, trigger, playerView) =>
        {
            Debug.Log($"Player: {player}. Message from server: {message}. Has trigger: {trigger.HasValue}. Has vertices: {(playerView != null ? 1 : 0)}");

            if (playerView != null)
            {
                PlayerView.Board = playerView.Board;
                PlayerView.PlayerInsects = playerView.PlayerInsects;
            }

            if (trigger.HasValue)
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

    public async Task LeaveQueueAsync()
    {
        await _hubConnection.InvokeAsync("LeaveQueue");
    }

    public async Task PutInsectAsync(InsectType insect, (int,int,int)? position)
    {
        await _hubConnection.InvokeAsync("JoinQueue", insect, position);
    }

    public async Task MoveInsectAsync()
    {
        throw new NotImplementedException();
        await _hubConnection.InvokeAsync("MoveInsectAsync");
    }
}
