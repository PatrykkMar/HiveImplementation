using HiveGame.BusinessLogic.Repositories;
using HiveGame.DataAccess.Repositories;
using System.Collections.Concurrent;
using System.Threading;

namespace HiveGame.Managers
{

    public interface IConnectionManager
    {
        public void AddPlayerConnection(string playerId, string connectionId);
        public Task<string?> RemovePlayerConnectionAsync(string connectionId);
        public void UpdatePlayerConnection(string playerId, string connectionId);
        public string? GetConnectionId(string playerId);
        public string? GetPlayerId(string connectionId);
        CancellationTokenSource AddDisconnectPlayerToken(string playerId);
        void RemoveDisconnectPlayerToken(string playerId, bool activateToken = true);
    }

    public class ConnectionManager : IConnectionManager
    {
        private readonly ConcurrentDictionary<string, string> PlayerConnectionDict = new();
        private readonly ConcurrentDictionary<string, CancellationTokenSource> DisconnectTokens = new();

        public ConnectionManager(IGameRepository gameRepository)
        {
            _gameRepository = gameRepository;
        }

        private IGameRepository _gameRepository { get; set; }
        public void AddPlayerConnection(string playerId, string connectionId)
        {
            PlayerConnectionDict.TryAdd(playerId, connectionId);
        }

        public string? GetPlayerFromConnection(string connectionId)
        {
            foreach (var pair in PlayerConnectionDict)
            {
                if (pair.Value == connectionId)
                {
                    return pair.Key;
                }
            }

            return null;
        }

        public async Task<string?> RemovePlayerConnectionAsync(string connectionId)
        {
            var playerId = GetPlayerFromConnection(connectionId);
            var keyToRemove = PlayerConnectionDict.Where(kvp => kvp.Value.Equals(connectionId)).Select(kvp => kvp.Key).FirstOrDefault();


            //quick solution when player exits the game
            var game = await _gameRepository.GetByPlayerIdAsync(keyToRemove);
            if(game != null) 
                await _gameRepository.RemoveAsync(game.Id);

            if(keyToRemove != null)
            {
                PlayerConnectionDict.TryRemove(keyToRemove, out _);
            }

            return playerId;
        }

        public string? GetConnectionId(string playerId) => PlayerConnectionDict.TryGetValue(playerId, out var connectionId) ? connectionId : null;
        public string? GetPlayerId(string connectionId) =>  PlayerConnectionDict.FirstOrDefault(kvp => kvp.Value == connectionId).Key;

        public void UpdatePlayerConnection(string playerId, string connectionId)
        {
            PlayerConnectionDict[playerId] = connectionId;
        }

        public CancellationTokenSource AddDisconnectPlayerToken(string playerId)
        {
            var cts = new CancellationTokenSource();
            if (!DisconnectTokens.TryAdd(playerId, cts))
            {
                DisconnectTokens[playerId]?.Cancel();
                DisconnectTokens[playerId] = cts;
            }
            return cts;
        }

        public void RemoveDisconnectPlayerToken(string playerId, bool activateToken = true)
        {
            CancellationTokenSource? token;
            if(activateToken && DisconnectTokens.TryGetValue(playerId, out token))
            {
                token.Cancel();
                token.Dispose();
            }
            DisconnectTokens.TryRemove(playerId, out _);
        }
    }
}
