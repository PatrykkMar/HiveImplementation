using HiveGame.BusinessLogic.Models;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;

namespace HiveGame.BusinessLogic.Managers
{
    public interface IWebSocketManager
    {

    }

    public sealed class WebSocketManager : IWebSocketManager
    {
        private readonly ConcurrentDictionary<string, Player> _connectedClients = new ConcurrentDictionary<string, Player>();

        public void AddClient(string playerName, string playerIP, WebSocket webSocket)
        {
            var client = new Player { PlayerName = playerName, PlayerIP = playerIP, WebSocket = webSocket };
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

        public async Task SendMessageAsync(string playerName, string message)
        {
            if (_connectedClients.TryGetValue(playerName, out var client))
            {
                var buffer = Encoding.UTF8.GetBytes(message);
                var segment = new ArraySegment<byte>(buffer);
                await client.WebSocket.SendAsync(segment, WebSocketMessageType.Text, true, System.Threading.CancellationToken.None);
            }
        }
    }
}
