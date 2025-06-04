using HiveGame.Core.Models;
using UnityEngine;

public class ServiceLocator
{
    private static ServiceLocator _instance;
    public static ServiceLocator Services
    {
        get
        {
            if (_instance == null)
            {
                Debug.Log("Creating ServiceLocator");
                _instance = new ServiceLocator();
            }
            return _instance;
        }
    }

    public IHubService HubService { get; private set; }
    public HttpService HttpService { get; private set; }
    public ClientStateMachine ClientStateMachine { get; private set; }
    public ConfigLoader ConfigLoader { get; private set; }
    public CurrentUser CurrentUser { get; private set; }
    public LogToFile LogToFile { get; private set; }
    public EventAggregator EventAggregator { get; private set; }

    private ServiceLocator()
    {
        InitializeServices();
        AddEvents();
    }

    private void InitializeServices()
    {
        ConfigLoader = new ConfigLoader();
        CurrentUser = new CurrentUser();
        EventAggregator = new EventAggregator();

        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            GameObject hubServiceObject = new GameObject("WebGlHubService");
            WebGlHubService webGLHubService = hubServiceObject.AddComponent<WebGlHubService>();
            webGLHubService.Initialize(ConfigLoader, EventAggregator);

            HubService = webGLHubService;
        }
        else
        {
            HubService = new HubService(ConfigLoader, EventAggregator);
        }

        ClientStateMachine = new ClientStateMachine();
        HttpService = new HttpService(ConfigLoader, EventAggregator, CurrentUser);
        LogToFile = new LogToFile();

    }

    private void AddEvents()
    {
        HubService.OnStateFromServerReceived += async (ClientState state) => 
        { 
            await ClientStateMachine.ChangeStateAsync(state); 
        };

        HttpService.OnTokenReceived += async (string token) => 
        { 
            CurrentUser.Token = token;
            await HubService.InitializeMatchmakingServiceAsync(token);
            await ClientStateMachine.ChangeStateAsync(ClientState.Connected);
        };
    }
}
