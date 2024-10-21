using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HubService
{
    public event Action<Trigger> OnTriggerReceived;

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
            .Build();

        _hubConnection.On<string, string, Trigger?, List<VertexDTO>, Dictionary<InsectType, int>>("ReceiveMessage", (player, message, trigger, board, playerInsects) =>
        {
            _mainThreadContext.Post(async _ =>
            {
                Debug.Log($"Player: {player}. Message from server: {message}. Has trigger: {trigger.HasValue}");


                if (trigger.HasValue)
                {
                    Debug.Log($"HubService: Got trigger");


                    var nextState = StateMachineConfiguration.CheckNextStateFromConfiguration(GameManager.GameState, trigger.Value);

                    if(nextState.HasValue) 
                    {
                        var nextStateScene = Scenes.GetSceneByState(nextState.Value);
                        if(nextStateScene != GameManager.GameScene)
                        {
                            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(nextStateScene);

                            while (!asyncLoad.isDone)
                            {
                                await Task.Yield();
                            }
                        }
                    }


                    OnTriggerReceived?.Invoke(trigger.Value);
                }


                if (playerInsects != null)
                {
                    Debug.Log($"HubService: Got player insects");
                    Board.Instance.SetPlayerInsects(playerInsects, invokeEvent: true);
                }

                if (board != null)
                {
                    Debug.Log($"HubService: Got board");
                    Board.Instance.SetBoard(board, invokeEvent: true);
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

    public async Task PutInsectAsync(InsectType insect, (int, int, int)? position)
    {
        await _hubConnection.InvokeAsync("PutInsect", insect, position);
    }

    public async Task PutFirstInsectAsync(InsectType insect)
    {
        await _hubConnection.InvokeAsync("PutFirstInsect", insect);
    }

    public async Task MoveInsectAsync()
    {
        throw new NotImplementedException();
        await _hubConnection.InvokeAsync("MoveInsectAsync");
    }
}