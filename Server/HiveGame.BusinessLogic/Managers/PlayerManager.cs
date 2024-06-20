using HiveGame.BusinessLogic.Models;
using HiveGame.BusinessLogic.Models.WebSocketModels;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;

namespace HiveGame.BusinessLogic.Managers
{
    public interface IPlayerManager
    {
        void AddClient(string playerName, string playerIP, WebSocket webSocket);

        void RemoveClient(string playerName);

        Player GetClient(string playerName);

        Task SendMessageAsync(string playerName, WebSocketMessage message);
    }

    public sealed class PlayerManager : IPlayerManager
    {
        private readonly ConcurrentDictionary<string, Player> _connectedClients = new ConcurrentDictionary<string, Player>();

        public void AddClient(string playerName, string playerIP, WebSocket webSocket)
        {
            var client = new Player { Nick = playerName, IP = playerIP, WebSocket = webSocket };
            _connectedClients.TryAdd(playerName, client);
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
