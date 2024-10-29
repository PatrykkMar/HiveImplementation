using Stateless;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    public class StateMachineConfiguration
    {
        public static StateMachine<ClientState, Trigger> CreateStateMachineWithConfiguration(ClientState currentState = ClientState.Disconnected)
        {
            var machine = new StateMachine<ClientState, Trigger>(currentState);

            machine.Configure(ClientState.Disconnected)
                .Permit(Trigger.ReceivedToken, ClientState.Connected)
                .PermitReentry(Trigger.Started);

            machine.Configure(ClientState.Connected)
                .Permit(Trigger.JoinedQueue, ClientState.WaitingForPlayers)
                .Permit(Trigger.FoundGamePlayerStarts, ClientState.InGamePlayerMove)
                .Permit(Trigger.FoundGameOpponentStarts, ClientState.InGameOpponentMove);

            machine.Configure(ClientState.WaitingForPlayers)
                .Permit(Trigger.LeftQueue, ClientState.Connected)
                .Permit(Trigger.FoundGamePlayerStarts, ClientState.InGamePlayerFirstMove)
                .Permit(Trigger.FoundGameOpponentStarts, ClientState.InGameOpponentMove);

            machine.Configure(ClientState.InGamePlayerFirstMove)
                .Permit(Trigger.OpponentMove, ClientState.InGameOpponentMove)
                .Permit(Trigger.EndGameConditionMet, ClientState.GameOver);

            machine.Configure(ClientState.InGamePlayerMove)
                .Permit(Trigger.OpponentMove, ClientState.InGameOpponentMove)
                .Permit(Trigger.EndGameConditionMet, ClientState.GameOver);

            machine.Configure(ClientState.InGameOpponentMove)
                .Permit(Trigger.PlayerMove, ClientState.InGamePlayerMove)
                .Permit(Trigger.PlayerFirstMove, ClientState.InGamePlayerFirstMove)
                .Permit(Trigger.EndGameConditionMet, ClientState.GameOver);

            machine.Configure(ClientState.GameOver);

        return machine;
        }


        public static ClientState? CheckNextStateFromConfiguration(ClientState state, Trigger trigger)
        {
            var machine = CreateStateMachineWithConfiguration(state);

            if (!machine.CanFire(trigger)) return null;

            machine.Fire(trigger);

            return machine.State;
        }

    }
