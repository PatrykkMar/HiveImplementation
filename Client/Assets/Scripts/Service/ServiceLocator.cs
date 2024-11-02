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

    public HubService HubService { get; private set; }
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

        ClientStateMachine = new ClientStateMachine();
        HubService = new HubService(ConfigLoader);
        HttpService = new HttpService(ConfigLoader, EventAggregator);
        LogToFile = new LogToFile();

    }

    private void AddEvents()
    {
        HubService.OnStateReceived += ClientStateMachine.Fire;
        HttpService.OnTokenReceived += async (string token) => 
        { 
            CurrentUser.Token = token;
            await HubService.InitializeMatchmakingServiceAsync(token);
            ClientStateMachine.Fire(ClientState.Connected);
        };
    }
}
