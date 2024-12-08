using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.AspNetCore.SignalR.Client;
using TMPro;
using Stateless;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using HiveGame.Core.Models;

public class ClientStateMachine
{
    public ClientStateMachine()
    {
        InitiateStateMachine();
    }

    public event Action<ClientState> OnStateChanged;
    public ClientState CurrentState { get; private set; }

    public void InitiateStateMachine()
    {

    }

    public void Fire(ClientState state)
    {
        HandleStateChanged(CurrentState, state);
        CurrentState = state;
    }


    public ClientState GetCurrentState()
    {
        return CurrentState;
    }

    public void SetForCurrentState()
    {
        OnStateChanged?.Invoke(GetCurrentState());
    }

    private void HandleStateChanged(ClientState oldState, ClientState newState)
    {
        var stateLifecycle = StateStrategyFactory.GetStrategy(oldState);

        stateLifecycle.OnStateExit();
        stateLifecycle.OnStateEntry();

        GameManager.GameState = newState;
        GameManager.GameScene = StateStrategyFactory.GetStrategy(newState).Scene;

        OnStateChanged?.Invoke(newState);
    }
}
