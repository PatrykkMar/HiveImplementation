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

    public ClientStateMachine(SceneService sceneService)
    {
        InitiateStateMachine();
        _sceneService = sceneService;
    }

    private StateMachine<ClientState, Trigger> _machine;

    public event Action<ClientState> OnStateChanged;

    public void InitiateStateMachine()
    {
        _machine = new StateMachine<ClientState, Trigger>(ClientState.Disconnected);

        _machine.Configure(ClientState.Disconnected)
            .Permit(Trigger.ReceivedToken, ClientState.Connected)
            .PermitReentry(Trigger.Started);

        _machine.Configure(ClientState.Connected)
            .Permit(Trigger.JoinedQueue, ClientState.WaitingForPlayers)
            .Permit(Trigger.FoundGamePlayerStarts, ClientState.InGamePlayerMove)
            .Permit(Trigger.FoundGameOpponentStarts, ClientState.InGameOpponentMove);

        _machine.Configure(ClientState.WaitingForPlayers)
            .Permit(Trigger.LeftQueue, ClientState.Connected)
            .Permit(Trigger.FoundGamePlayerStarts, ClientState.InGamePlayerFirstMove)
            .Permit(Trigger.FoundGameOpponentStarts, ClientState.InGameOpponentMove);

        _machine.Configure(ClientState.InGamePlayerFirstMove)
            .Permit(Trigger.PlayerMadeMove, ClientState.InGameOpponentMove);

        _machine.Configure(ClientState.InGamePlayerMove)
            .Permit(Trigger.PlayerMadeMove, ClientState.InGameOpponentMove);

        _machine.Configure(ClientState.InGameOpponentMove)
            .Permit(Trigger.OpponentMadeMove, ClientState.InGamePlayerMove)
            .Permit(Trigger.PlayerFirstMove, ClientState.InGamePlayerFirstMove);

        _machine.OnTransitioned(transition =>
            {
                Debug.Log($"{transition.Source} -[{transition.Trigger}]-> {transition.Destination}");
                HandleStateChanged(transition.Destination);
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

    private void HandleStateChanged(ClientState newState)
    {
        if(_sceneService.ChangeSceneForStateIfNecessary(newState))
        {
            //making the event to be emitted after loading the scene (executing OnEnable/OnDisable of all scripts)
            void sceneLoadedHandler(Scene scene, LoadSceneMode mode)
            {
                OnStateChanged?.Invoke(newState);
                SceneManager.sceneLoaded -= sceneLoadedHandler;
            }

            SceneManager.sceneLoaded += sceneLoadedHandler;
        }
        else
        {
            OnStateChanged?.Invoke(newState);
        }
    }
}
