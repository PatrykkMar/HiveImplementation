using System;
using System.Threading;
using System.Threading.Tasks;
using HiveGame.Core.Models;
using HiveGame.Core.Models.Requests;
using Microsoft.AspNetCore.SignalR.Client;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HubService : IHubService
{
    public event Action<ClientState> OnStateReceived;

    private readonly ConfigLoader _configLoader;
    private SynchronizationContext _mainThreadContext;
    private readonly EventAggregator _eventAggregator;
    public HubService(ConfigLoader configLoader, EventAggregator eventAggregator)
    {
        _configLoader = configLoader;
        _mainThreadContext = SynchronizationContext.Current;
        _eventAggregator = eventAggregator;
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

        _hubConnection.On<ReceiveMessageRequest>("ReceiveMessage", (request) =>
        {
            _mainThreadContext.Post(async _ =>
            {
                Debug.Log($"Player: {request.playerId}. Message from server: {request.message}. Has trigger: {request.state.HasValue}");


                if (request.state.HasValue)
                {
                    Debug.Log($"HubService: Got state");

                    if(request.state.HasValue) 
                    {
                        var nextStateScene = Scenes.GetSceneByState(request.state.Value);
                        if(nextStateScene != GameManager.GameScene)
                        {
                            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(nextStateScene);

                            while (!asyncLoad.isDone)
                            {
                                await Task.Yield();
                            }
                        }
                    }

                    OnStateReceived?.Invoke(request.state.Value);
                }


                if (request.playerView?.PlayerInsects != null)
                {
                    Debug.Log($"HubService: Got player insects");
                    Board.Instance.SetPlayerInsects(request.playerView.PlayerInsects, invokeEvent: true);
                }

                if (request.playerView?.Board != null)
                {
                    Debug.Log($"HubService: Got board");
                    Board.Instance.SetBoardFromDTO(request.playerView.Board, invokeEvent: true);
                }
            }, null);
        });

        _hubConnection.On<string>("ReceiveError", (error) =>
        {
            _mainThreadContext.Post(async _ =>
            {
                _eventAggregator.InvokeMinorInformationTextReceived(error);
            }, null);
        });

        await _hubConnection.StartAsync();
    }

    public async Task JoinQueueAsync(string playerNick)
    {
        await _hubConnection.InvokeAsync("JoinQueue", playerNick);
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
        ServiceLocator.Services.EventAggregator.InvokeMinorInformationTextReceived("Connection closed", 3f);
    }

    private async Task OnReconnecting(Exception ex)
    {
        Debug.LogError("Attempting to reconnect...");
        ServiceLocator.Services.EventAggregator.InvokeMinorInformationTextReceived("Attempting to reconnect...", null);
    }

    private async Task OnReconnected(string connectionId)
    {
        ServiceLocator.Services.EventAggregator.InvokeMinorInformationTextReceived("Successfully reconnected", 3f);
    }
}