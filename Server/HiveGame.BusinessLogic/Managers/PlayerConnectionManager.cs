using HiveGame.BusinessLogic.Models;
using HiveGame.BusinessLogic.Models.WebSocketModels;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;

namespace HiveGame.BusinessLogic.Managers
{
    public interface IPlayerConnectionManager
    {
        Player AddClient(string playerName, WebSocket webSocket);

        void RemoveClient(string playerName);

        Player GetClient(string playerName);
        List<Player> GetClients();

        Task SendMessageAsync(string playerName, WebSocketMessage message);
    }

    public sealed class PlayerConnectionManager : IPlayerConnectionManager
    {
        private readonly ConcurrentDictionary<string, Player> _connectedClients = new ConcurrentDictionary<string, Player>();

        public Player AddClient(string playerName, WebSocket webSocket)
        {
            var client = new Player { Nick = playerName, WebSocket = webSocket };
            _connectedClients.TryAdd(playerName, client);
            return client;
        }

        public void RemoveClient(string playerName)
        {
            _connectedClients.TryRemove(playerName, out _);
        }

        public Player GetClient(string playerName)
        {
            _connectedClients.TryGetValue(playerName, out var client);
            return client;
        }

        public List<Player> GetClients()
        {
            return _connectedClients.Values.ToList();
        }

        public async Task SendMessageAsync(string playerName, WebSocketMessage message)
        {
            if (_connectedClients.TryGetValue(playerName, out var client))
            {
                var jsonMessage = JsonConvert.SerializeObject(message);
                var buffer = Encoding.UTF8.GetBytes(jsonMessage);
                var segment = new ArraySegment<byte>(buffer);
                await client.WebSocket.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }
}
