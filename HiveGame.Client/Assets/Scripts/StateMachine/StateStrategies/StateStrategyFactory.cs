using System.Collections.Generic;
using System;
using HiveGame.Core.Models;

public static class StateStrategyFactory
{
    private static readonly Dictionary<ClientState, IStateStrategy> _strategies;

    static StateStrategyFactory()
    {
        _strategies = new Dictionary<ClientState, IStateStrategy>
        {
            { ClientState.Disconnected, new DisconnectedStateStrategy() },
            { ClientState.Connected, new ConnectedStateStrategy() },
            { ClientState.WaitingInQueue, new WaitingInQueueStateStrategy() },
            { ClientState.PendingMatchConfirmation, new PendingMatchConfirmationStateStrategy() },
            { ClientState.InGamePlayerMove, new InGamePlayerMoveStateStrategy() },
            { ClientState.InGamePlayerFirstMove, new InGamePlayerFirstMoveStateStrategy() },
            { ClientState.InGameOpponentMove, new InGameOpponentMoveStateStrategy() },
            { ClientState.GameOver, new GameOverStateStrategy() }
        };
    }

    public static IStateStrategy GetStrategy(ClientState state)
    {
        if (_strategies.TryGetValue(state, out var strategy))
        {
            return strategy;
        }

        throw new ArgumentException($"Strategy for state {state} not implemented");
    }

    public static IStateStrategy GetCurrentStateStrategy()
    {
        return GetStrategy(ServiceLocator.Services.ClientStateMachine.GetCurrentState());
    }
}