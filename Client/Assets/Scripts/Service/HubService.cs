using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class HubService
{
    public event Action<ClientState> OnStateReceived;

    private readonly ConfigLoader _configLoader;
    private SynchronizationContext _mainThreadContext;
    public HubService(ConfigLoader configLoader)
    {
        _configLoader = configLoader;
        _mainThreadContext = SynchronizationContext.Current;
    }

    private HubConnection _hubConnection;

    private string _serverUrl
    {
        get
        {
            return _configLoader.GetConfigValue(ConfigLoaderConsts.MatchmakingHubUrlKey);
        }
    }


    public async Task InitializeMatchmakingServiceAsync(string token)
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl(_serverUrl, options =>
            {
                options.AccessTokenProvider = () => Task.FromResult(token);
            })
            .WithAutomaticReconnect(new TimeSpan[]
            {
                TimeSpan.FromSeconds(0),
                TimeSpan.FromSeconds(2),
                TimeSpan.FromSeconds(10), 
                TimeSpan.FromSeconds(30),
            })
            .Build();

        _hubConnection.Closed += OnConnectionClosed;
        _hubConnection.Reconnecting += OnReconnecting;
        _hubConnection.Reconnected += OnReconnected;

        _hubConnection.On<string, string, ClientState?, PlayerViewDTO>("ReceiveMessage", (player, message, state, playerView) =>
        {
            _mainThreadContext.Post(async _ =>
            {
                Debug.Log($"Player: {player}. Message from server: {message}. Has trigger: {state.HasValue}");


                if (state.HasValue)
                {
                    Debug.Log($"HubService: Got state");

                    if(state.HasValue) 
                    {
                        var nextStateScene = Scenes.GetSceneByState(state.Value);
                        if(nextStateScene != GameManager.GameScene)
                        {
                            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(nextStateScene);

                            while (!asyncLoad.isDone)
                            {
                                await Task.Yield();
                            }
                        }
                    }

                    OnStateReceived?.Invoke(state.Value);
                }


                if (playerView?.PlayerInsects != null)
                {
                    Debug.Log($"HubService: Got player insects");
                    Board.Instance.SetPlayerInsects(playerView.PlayerInsects, invokeEvent: true);
                }

                if (playerView?.Board != null)
                {
                    Debug.Log($"HubService: Got board");
                    Board.Instance.SetBoardFromDTO(playerView.Board, invokeEvent: true);
                }
            }, null);
        });

        await _hubConnection.StartAsync();
    }

    public async Task JoinQueueAsync()
    {
        await _hubConnection.InvokeAsync("JoinQueue");
    }

    public async Task LeaveQueueAsync()
    {
        await _hubConnection.InvokeAsync("LeaveQueue");
    }

    public async Task PutInsectAsync(InsectType insect, (int, int, int) position)
    {
        int[] point = new int[3] { position.Item1, position.Item2, position.Item3 };
        await _hubConnection.InvokeAsync("PutInsect", insect, point);
    }

    public async Task PutFirstInsectAsync(InsectType insect)
    {
        await _hubConnection.InvokeAsync("PutFirstInsect", insect);
    }

    public async Task MoveInsectAsync((int, int, int) moveFrom, (int, int, int) moveTo)
    {
        int[] moveFromPoint = new int[3] { moveFrom.Item1, moveFrom.Item2, moveFrom.Item3 };
        int[] moveToPoint = new int[3] { moveTo.Item1, moveTo.Item2, moveTo.Item3 };
        await _hubConnection.InvokeAsync("MoveInsect", moveFromPoint, moveToPoint);
    }

    private async Task OnConnectionClosed(Exception ex)
    {
        Debug.LogError($"Connection closed: {ex?.Message}");
    }

    private async Task OnReconnecting(Exception ex)
    {
        Debug.LogError("Attempting to reconnect...");
    }

    private async Task OnReconnected(string connectionId)
    {
        Debug.LogError($"");
    }
}