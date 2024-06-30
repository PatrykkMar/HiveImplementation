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
        //void AddClient(string playerName, string playerIP, WebSocket webSocket);

        //void RemoveClient(string playerName);

        //Player GetClient(string playerName);
    }

    public class PlayerManager : IPlayerManager
    {
        //private static ConcurrentDictionary<string, string> ConnectedClients = new ConcurrentDictionary<string, string>();

        //public static PlayerManager Instance => lazy.Value;

        //private PlayerManager()
        //{
        //    ConnectedClients = new ConcurrentDictionary<string, (string Id, string UserName)>();
        //}

        //public void AddClient(string connectionId, string id, string userName)
        //{
        //    ConnectedClients.TryAdd(connectionId, (id, userName));
        //}

        //public void RemoveClient(string connectionId)
        //{
        //    ConnectedClients.TryRemove(connectionId, out _);
        //}

        //public IEnumerable<(string Id, string UserName)> GetAllClients()
        //{
        //    return ConnectedClients.Values;
        //}
    }
}
