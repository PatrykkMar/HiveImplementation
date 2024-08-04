using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.AspNetCore.SignalR.Client;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private HubService _service;
    [SerializeField] private TokenService _token;
    [SerializeField] private ClientStateMachine _stateMachine;

    public static bool Initiated = false;

    async void Awake()
    {
        Debug.Log("Scene started");
        if (Initiated == false)
        {
            _stateMachine.InitiateStateMachine();
            StartCoroutine(_token.GetToken(true));
            await _service.InitializeMatchmakingServiceAsync();
            Initiated = true;
        }
        _stateMachine.SetForCurrentState();
    }
}
