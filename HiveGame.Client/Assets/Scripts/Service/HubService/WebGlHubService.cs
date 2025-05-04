using HiveGame.Core.Models;
using HiveGame.Core.Models.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;


public class WebGlHubService : MonoBehaviour, IHubService
{
    public event Action<ClientState> OnStateReceived;

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
    private static extern void JoinQueue();

    [DllImport("__Internal")]
    private static extern void LeaveQueue();

    [DllImport("__Internal")]
    private static extern void PutInsect(string insect, int x, int y, int z);

    [DllImport("__Internal")]
    private static extern void PutFirstInsect(string insect);

    [DllImport("__Internal")]
    private static extern void MoveInsect(int fromX, int fromY, int fromZ, int toX, int toY, int toZ);

    public async Task InitializeMatchmakingServiceAsync(string token)
    {
        Debug.Log("WebGlHubService - connecting to: " + _configLoader.GetConfigValue(ConfigLoaderConsts.MatchmakingHubUrlKey) + "with token: " + token);
        InitializeSignalR(_configLoader.GetConfigValue(ConfigLoaderConsts.MatchmakingHubUrlKey), token);
    }

    public async Task JoinQueueAsync()
    {
        JoinQueue();
    }

    public async Task LeaveQueueAsync()
    {
        LeaveQueue();
    }

    public async Task PutInsectAsync(InsectType insect, (int, int, int) position)
    {
        PutInsect(insect.ToString(), position.Item1, position.Item2, position.Item3);
    }

    public async Task PutFirstInsectAsync(InsectType insect)
    {
        PutFirstInsect(insect.ToString());
    }

    public async Task MoveInsectAsync((int, int, int) moveFrom, (int, int, int) moveTo)
    {
        MoveInsect(moveFrom.Item1, moveFrom.Item2, moveFrom.Item3, moveTo.Item1, moveTo.Item2, moveTo.Item3);
    }

    public void ReceiveMessage(string json)
    {
        var request = JsonUtility.FromJson<ReceiveMessageRequest>(json);

        Debug.Log(request.playerId);
        Debug.Log(request.message);

        try
        {
            OnStateReceived?.Invoke(request.state.Value);

        }
        catch (Exception e) {
            Debug.Log("Not found Value xd");
        }
    }
}