using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.AspNetCore.SignalR.Client;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private HubService _service;
    [SerializeField] private TokenService _token;
    [SerializeField] private ClientStateMachine _stateMachine;

    public static bool Initiated = false;

    async void Awake()
    {
        if(Initiated == false)
        {
            _stateMachine.InitiateStateMachine();
            _stateMachine.Fire(Trigger.Started);
            StartCoroutine(_token.GetToken(true));
            await _service.InitializeMatchmakingServiceAsync();
            Initiated = true;
        }
    }
}