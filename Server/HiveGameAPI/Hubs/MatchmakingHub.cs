using HiveGame.BusinessLogic.Models.Requests;
using HiveGame.BusinessLogic.Services;
using HiveGame.BusinessLogic.Utils;
using HiveGameAPI.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace HiveGame.Hubs
{
    public class MatchmakingHub : Hub
    {
        private readonly ITokenUtils _utils;
        private readonly IMatchmakingService _service;

        public MatchmakingHub(ITokenUtils utils, IMatchmakingService service)
        {
            _utils = utils;
            _service = service;
        }

        private static ConcurrentDictionary<string, string> ConnectedClients = new ConcurrentDictionary<string, string>(); //client, connection

        [Authorize]
        public override async Task OnConnectedAsync()
        {
            string connectionId = Context.ConnectionId;
            string clientId = GetPlayerIdFromToken();
            ConnectedClients.TryAdd(clientId, connectionId);
            await base.OnConnectedAsync();
            await SendMessageAsync($"user: {clientId}", "You connected to the server hub");
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            string connectionId = Context.ConnectionId;
            ConnectedClients.TryRemove(connectionId, out _);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessageAsync(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task CreateJwtTokenAsync(string clientId)
        {
            var jwtToken = _utils.CreateToken(clientId);

            await Clients.Caller.SendAsync("ReceiveToken", jwtToken);
        }

        [Authorize]
        public async Task JoinQueue() //Request
        {

            var playerId = GetPlayerIdFromToken();

            var players = _service.JoinQueue(playerId);

            if(players != null)
            {
                await Clients.Clients(players.Select(x => ConnectedClients[x])).SendAsync("ReceiveMessage", playerId, "Player found");
            }
            else
            {
                await Clients.Caller.SendAsync("ReceiveMessage", playerId, "Waiting for player");
            }
        }

        private string GetPlayerIdFromToken()
        {
            var playerIdClaim = Context.User?.FindFirst("PlayerId");
            return playerIdClaim?.Value;
        }
    }
}