using HiveGame.Core.Models;
using HiveGame.Core.Models.Requests;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;


public class WebGlHubService : MonoBehaviour, IHubService
{
    public event Action<ClientState> OnStateFromServerReceived;

    private ConfigLoader _configLoader;
    private SynchronizationContext _mainThreadContext;
    private EventAggregator _eventAggregator;
    public WebGlHubService(ConfigLoader configLoader, EventAggregator eventAggregator)
    {
        _configLoader = configLoader;
        _mainThreadContext = SynchronizationContext.Current;
        _eventAggregator = eventAggregator;
    }

    public void Initialize(ConfigLoader configLoader, EventAggregator eventAggregator)
    {
        _configLoader = configLoader;
        _mainThreadContext = SynchronizationContext.Current;
        _eventAggregator = eventAggregator;
    }

    private void Awake()
    {
        Debug.Log("WebGlHubService added to scene: " + gameObject.name);
        DontDestroyOnLoad(gameObject);
    }

    [DllImport("__Internal")]
    private static extern void InitializeSignalR(string serverUrl, string token);

    [DllImport("__Internal")]
    private static extern void JoinQueue(string nick);

    [DllImport("__Internal")]
    private static extern void LeaveQueue();

    [DllImport("__Internal")]
    private static extern void PutInsect(InsectType insect, int x, int y, int z);

    [DllImport("__Internal")]
    private static extern void PutFirstInsect(InsectType insect);

    [DllImport("__Internal")]
    private static extern void MoveInsect(int fromX, int fromY, int fromZ, int toX, int toY, int toZ);
    [DllImport("__Internal")]
    private static extern void ConfirmGame();
    [DllImport("__Internal")]
    private static extern void FinishGame();

    public async Task InitializeMatchmakingServiceAsync(string token)
    {
        Debug.Log("WebGlHubService - connecting to: " + _configLoader.GetConfigValue(ConfigLoaderConsts.MatchmakingHubUrlKey) + "with token: " + token);
        InitializeSignalR(_configLoader.GetConfigValue(ConfigLoaderConsts.MatchmakingHubUrlKey), token);
    }

    public async Task JoinQueueAsync(string nick)
    {
        JoinQueue(nick);
    }

    public async Task LeaveQueueAsync()
    {
        LeaveQueue();
    }

    public async Task PutInsectAsync(InsectType insect, (int, int, int) position)
    {
        PutInsect(insect, position.Item1, position.Item2, position.Item3);
    }

    public async Task PutFirstInsectAsync(InsectType insect)
    {
        PutFirstInsect(insect);
    }

    public async Task MoveInsectAsync((int, int, int) moveFrom, (int, int, int) moveTo)
    {
        MoveInsect(moveFrom.Item1, moveFrom.Item2, moveFrom.Item3, moveTo.Item1, moveTo.Item2, moveTo.Item3);
    }

    public async Task ConfirmGameAsync()
    {
        ConfirmGame();
    }

    public async Task FinishGameAsync()
    {
        FinishGame();
    }

    public void ReceiveMessage(string json)
    {
        _mainThreadContext.Post(async _ =>
        {

            var unityRequest = JsonUtility.FromJson<ReceiveMessageRequestForUnitySerialization>(json);

            var request = ReceiveMessageRequestForUnitySerialization.ConvertToOriginal(unityRequest);

            Debug.Log($"Player: {request.playerId}. Message from server: {request.message}. Has trigger: {request.state.HasValue}");


            if (request.state.HasValue)
            {
                Debug.Log($"HubService: Got state");

                if (request.state.HasValue)
                {
                    var nextStateScene = Scenes.GetSceneByState(request.state.Value);
                    if (nextStateScene != GameManager.GameScene)
                    {
                        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(nextStateScene);

                        while (!asyncLoad.isDone)
                        {
                            await Task.Yield();
                        }
                    }
                }

                OnStateFromServerReceived?.Invoke(request.state.Value);
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

            if (!string.IsNullOrEmpty(request.message))
            {
                Debug.Log($"HubService: Minor information message: " + request.message); 
                _eventAggregator.InvokeMinorInformationTextReceived(request.message, unstoppableTime: 5);
            }
        }, null);
    }
}