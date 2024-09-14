using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        InitializeServiceLocator();
    }

    private void Start()
    {
        ServiceLocator.Services.ClientStateMachine.Fire(Trigger.Started);

        StartCoroutine(ServiceLocator.Services.HttpService.GetToken());
    }

    private void InitializeServiceLocator()
    {
        var services = ServiceLocator.Services;
        Debug.Log("ServiceLocator initialized.");
    }
}