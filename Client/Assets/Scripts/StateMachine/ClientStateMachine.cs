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
                _ui.SetStateToChange(transition.Destination);
            }
        );
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
