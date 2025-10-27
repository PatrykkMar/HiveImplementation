using UnityEngine;
using System.Collections;
using HiveGame.Core.Models;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public static string GameScene = null;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // usuñ duplikat
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeServiceLocator();
    }

    async void Start()
    {
        await ServiceLocator.Services.ClientStateMachine.ChangeStateAsync(ClientState.Disconnected);

        StartCoroutine(ServiceLocator.Services.HttpService.GetTokenLoop());
    }

    private void InitializeServiceLocator()
    {
        var services = ServiceLocator.Services;
        Debug.Log("ServiceLocator is initialized.");
    }
}