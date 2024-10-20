using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.AspNetCore.SignalR.Client;
using TMPro;
using Stateless;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class ClientStateMachine
{
    private readonly SceneService _sceneService;

    public ClientStateMachine()
    {
        InitiateStateMachine();
    }

    private StateMachine<ClientState, Trigger> _machine;

    public event Action<ClientState> OnStateChanged;

    public void InitiateStateMachine()
    {
        _machine = StateMachineConfiguration.CreateStateMachineWithConfiguration();

        _machine.OnTransitioned(transition =>
            {
                Debug.Log($"{transition.Source} -[{transition.Trigger}]-> {transition.Destination}");
                HandleStateChanged(transition.Source, transition.Destination);
            }
        );
    }

    public void Fire(Trigger trigger)
    {
        _machine.Fire(trigger);
    }


    public ClientState GetCurrentState()
    {
        return _machine.State;
    }

    public void SetForCurrentState()
    {
        OnStateChanged?.Invoke(GetCurrentState());
    }

    private void HandleStateChanged(ClientState oldState, ClientState newState)
    {
        StateStrategyFactory.GetStrategy(oldState).OnEntry();
        StateStrategyFactory.GetStrategy(newState).OnEntry();

        GameManager.GameState = newState;
        GameManager.GameScene = StateStrategyFactory.GetStrategy(newState).Scene;

        OnStateChanged?.Invoke(newState);
    }
}
