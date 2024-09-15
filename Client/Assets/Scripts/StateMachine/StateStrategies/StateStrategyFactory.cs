using System.Collections.Generic;
using System;

public static class StateStrategyFactory
{
    private static readonly Dictionary<ClientState, IStateStrategy> _strategies;

    static StateStrategyFactory()
    {
        _strategies = new Dictionary<ClientState, IStateStrategy>
        {
            { ClientState.Disconnected, new DisconnectedStateStrategy() },
            { ClientState.Connected, new ConnectedStateStrategy() },
            { ClientState.WaitingForPlayers, new WaitingForPlayersStateStrategy() },
            { ClientState.InGamePlayerMove, new InGamePlayerMoveStateStrategy() },
            { ClientState.InGamePlayerFirstMove, new InGamePlayerFirstMoveStateStrategy() },
            { ClientState.InGameOpponentMove, new InGameOpponentMoveStateStrategy() }
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
}