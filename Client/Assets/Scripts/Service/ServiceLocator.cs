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

    private ServiceLocator()
    {
        InitializeServices();
        AddEvents();
    }

    private void InitializeServices()
    {
        ConfigLoader = new ConfigLoader();
        ClientStateMachine = new ClientStateMachine();
        HubService = new HubService(ConfigLoader);
        HttpService = new HttpService(ConfigLoader);
        CurrentUser = new CurrentUser();
        LogToFile = new LogToFile();
    }

    private void AddEvents()
    {
        HubService.OnTriggerReceived += ClientStateMachine.Fire;
        HttpService.OnTokenReceived += async (string token) => 
        { 
            CurrentUser.Token = token;
            ClientStateMachine.Fire(Trigger.ReceivedToken);
            await HubService.InitializeMatchmakingServiceAsync(token);
        };
    }
}
