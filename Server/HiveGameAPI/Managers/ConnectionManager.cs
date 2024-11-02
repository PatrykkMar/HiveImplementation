using System.Collections.Concurrent;

namespace HiveGame.Managers
{

    public interface IConnectionManager
    {
        public void AddPlayerConnection(string playerId, string connectionId);
        public void RemovePlayerConnection(string connectionId);
        public string? GetConnectionId(string playerId);
    }

    public class ConnectionManager : IConnectionManager
    {
        private static readonly ConcurrentDictionary<string, string> PlayerConnectionDict = new();

        public void AddPlayerConnection(string playerId, string connectionId)
        {
            PlayerConnectionDict.TryAdd(playerId, connectionId);
        }

        public void RemovePlayerConnection(string connectionId)
        {
            var keysToRemove = PlayerConnectionDict.Where(kvp => kvp.Value.Equals(connectionId)).Select(kvp => kvp.Key).ToList();
            foreach (var key in keysToRemove)
            {
                PlayerConnectionDict.TryRemove(key, out _);
            }
        }

        public string? GetConnectionId(string playerId) => PlayerConnectionDict.TryGetValue(playerId, out var connectionId) ? connectionId : null;
    }
}
