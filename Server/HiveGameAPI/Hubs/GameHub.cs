using HiveGame.BusinessLogic.Models.Requests;
using HiveGame.BusinessLogic.Services;
using HiveGame.BusinessLogic.Utils;
using HiveGameAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace HiveGame.Hubs
{
    public class GameHub : Hub
    {
        private readonly ITokenUtils _utils;

        public GameHub(ITokenUtils utils)
        {
            _utils = utils;
        }

        private static ConcurrentDictionary<string, string> ConnectedClients = new ConcurrentDictionary<string, string>(); //connection, client

        public override async Task OnConnectedAsync()
        {
            string connectionId = Context.ConnectionId;
            string clientId = Guid.NewGuid().ToString();
            ConnectedClients.TryAdd(connectionId, clientId);
            await base.OnConnectedAsync();
            await CreateJwtTokenAsync(clientId);
            await SendMessage("test", "test");
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            string connectionId = Context.ConnectionId;
            ConnectedClients.TryRemove(connectionId, out _);
            await Clients.All.SendAsync("UpdateClientList", ConnectedClients.Values);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task CreateJwtTokenAsync(string clientId)
        {
            var jwtToken = _utils.CreateToken(clientId);

            await Clients.Caller.SendAsync("ReceiveToken", jwtToken);
        }

        public async Task JoinQueue(string a)
        {

        }
    }
}