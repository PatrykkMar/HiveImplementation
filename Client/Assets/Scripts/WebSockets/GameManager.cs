using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.AspNetCore.SignalR.Client;
using TMPro;

public class GameManager : MonoBehaviour
{
    public Text informationText;
    [SerializeField] private MatchmakingHubService _service;
    [SerializeField] private TokenService _token;
    [SerializeField] private ClientStateMachine _stateMachine;

    async void Awake()
    {
        _stateMachine.InitiateStateMachine();
        _stateMachine.Fire(Trigger.Started);
        StartCoroutine(_token.GetToken(true));
        await _service.InitializeMatchmakingServiceAsync();
        //StartCoroutine(_token.GetToken());
    }
}