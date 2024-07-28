using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.AspNetCore.SignalR.Client;
using TMPro;
using Stateless;

public class ClientStateMachine : MonoBehaviour
{
    private StateMachine<ClientState, Trigger> _machine;
    public Button[] buttons;
    [SerializeField] private UIManager _ui;


    public void InitiateStateMachine()
    {
        if(_machine != null)
        {
            Debug.LogWarning("State machine was initiated");
        }

        _machine = new StateMachine<ClientState, Trigger>(ClientState.Nothing);

        _machine.Configure(ClientState.Nothing)
            .Permit(Trigger.ReceivedToken, ClientState.Connected)
            .PermitReentry(Trigger.Started);

        _machine.Configure(ClientState.Connected)
            .Permit(Trigger.JoinedQueue, ClientState.WaitingForPlayers);

        _machine.Configure(ClientState.WaitingForPlayers)
            .Permit(Trigger.LeftQueue, ClientState.Connected)
            .Permit(Trigger.FoundGame, ClientState.InGame);

        _machine.Configure(ClientState.InGame);

        _machine.OnTransitioned(transition =>
        {
            Debug.Log($"{transition.Source} -[{transition.Trigger}]-> {transition.Destination}");
            _ui.SetStateToChange(transition.Destination);
        });
    }

    public void Fire(Trigger trigger)
    {
        _machine.Fire(trigger);
    }
}
