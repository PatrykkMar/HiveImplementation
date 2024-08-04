using HiveGame.BusinessLogic.Models.Insects;
using HiveGame.BusinessLogic.Models.Requests;
using HiveGame.BusinessLogic.Services;
using HiveGame.BusinessLogic.Utils;
using HiveGame.Models;
using HiveGameAPI.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace HiveGame.Hubs
{
    public class GameHub : Hub
    {
        private readonly ITokenUtils _utils;
        private readonly IMatchmakingService _matchmakingService;
        private readonly IHiveGameService _gameService;

        public GameHub(ITokenUtils utils, IMatchmakingService matchmakingService, IHiveGameService gameService)
        {
            _utils = utils;
            _matchmakingService = matchmakingService;
            _gameService = gameService;
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

            var keysToRemove = ConnectedClients.Where(kvp => kvp.Value.Equals(connectionId)).Select(kvp => kvp.Key).ToList();

            foreach (var key in keysToRemove)
            {
                ConnectedClients.TryRemove(connectionId, out _);
            }

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
        public async Task JoinQueue()
        {

            var playerId = GetPlayerIdFromToken();

            var players = _matchmakingService.JoinQueue(playerId);

            if(players != null)
            {
                await Clients.Clients(players.Select(x => ConnectedClients[x])).SendAsync("ReceiveMessage", playerId, "Player found", Trigger.FoundGame, null);
            }
            else
            {
                await Clients.Caller.SendAsync("ReceiveMessage", playerId, "Waiting for player", Trigger.JoinedQueue, null);
            }
        }

        [Authorize]
        public async Task LeaveQueue()
        {

            var playerId = GetPlayerIdFromToken();

            _matchmakingService.LeaveQueue(playerId);

            await Clients.Caller.SendAsync("ReceiveMessage", playerId, "Left the queue", Trigger.LeftQueue, null);
        }

        [Authorize]
        public async Task PutFirstInsect(InsectType type)
        {
            var request = new PutFirstInsectRequest()
            {
                InsectToPut = type,
                PlayerId = GetPlayerIdFromToken()
            };

            _gameService.PutFirstInsect(request);
        }

        private string GetPlayerIdFromToken()
        {
            var playerIdClaim = Context.User?.FindFirst("PlayerId");
            return playerIdClaim?.Value;
        }
    }
}