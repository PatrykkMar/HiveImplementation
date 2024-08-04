using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.AspNetCore.SignalR.Client;
using TMPro;
using Stateless;

public class ClientStateMachine : MonoBehaviour
{
    private static StateMachine<ClientState, Trigger> _machine;
    public Button[] buttons;
    [SerializeField] private UIManager _ui;


    public void InitiateStateMachine()
    {
        if(_machine != null)
        {
            Debug.LogWarning("State machine was initiated");
            return;
        }

        _machine = new StateMachine<ClientState, Trigger>(ClientState.Disconnected);

        _machine.Configure(ClientState.Disconnected)
            .OnEntry(() => _ui.ConfigureUIForState(ClientState.Disconnected))
            .Permit(Trigger.ReceivedToken, ClientState.Connected)
            .PermitReentry(Trigger.Started);

        _machine.Configure(ClientState.Connected)
            .OnEntry(() => _ui.ConfigureUIForState(ClientState.Connected))
            .Permit(Trigger.JoinedQueue, ClientState.WaitingForPlayers)
            .Permit(Trigger.FoundGame, ClientState.InGame);

        _machine.Configure(ClientState.WaitingForPlayers)
            .OnEntry(() => _ui.ConfigureUIForState(ClientState.WaitingForPlayers))
            .Permit(Trigger.LeftQueue, ClientState.Connected)
            .Permit(Trigger.FoundGame, ClientState.InGame);

        _machine.Configure(ClientState.InGame)
            .OnEntry(() => _ui.ConfigureUIForState(ClientState.InGame));

        _machine.OnTransitioned(transition => Debug.Log($"{transition.Source} -[{transition.Trigger}]-> {transition.Destination}"));
    }

    public void SetForCurrentState()
    {
        _ui.ConfigureUIForState(_machine.State);
    }

    public void Fire(Trigger trigger)
    {
        _machine.Fire(trigger);
    }
}
