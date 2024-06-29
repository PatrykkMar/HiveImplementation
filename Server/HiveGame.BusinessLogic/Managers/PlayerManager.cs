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
            throw new NotImplementedException(); 
        }

        public void RemoveClient(string playerName)
        {
            throw new NotImplementedException();
        }

        public Player GetClient(string playerName)
        {
            throw new NotImplementedException();
        }

        public async Task SendMessageAsync(string playerName, WebSocketMessage message)
        {
            throw new NotImplementedException();
        }
    }
}
