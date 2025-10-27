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

    public async Task ChangeStateAsync(ClientState state)
    {
        await HandleStateChangedAsync(CurrentState, state);
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

    private async Task HandleStateChangedAsync(ClientState oldState, ClientState newState)
    {
        await ChangeSceneAsync(newState);

        var stateLifecycle = StateStrategyFactory.GetStrategy(oldState);

        stateLifecycle.OnStateExit();
        stateLifecycle.OnStateEntry();

        GameManager.GameScene = StateStrategyFactory.GetStrategy(newState).Scene;

        OnStateChanged?.Invoke(newState);
    }

    private async Task ChangeSceneAsync(ClientState state)
    {
        var nextStateScene = Scenes.GetSceneByState(state);
        if (nextStateScene != GameManager.GameScene)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(nextStateScene);

            while (!asyncLoad.isDone)
            {
                await Task.Yield();
            }
        }
    }
}
