using HiveGame.BusinessLogic.Models;
using HiveGame.BusinessLogic.Models.Game;
using HiveGame.BusinessLogic.Models.Insects;
using HiveGame.BusinessLogic.Models.Requests;
using HiveGame.BusinessLogic.Services;
using HiveGame.BusinessLogic.Utils;
using HiveGame.Models;
using HiveGameAPI.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json.Linq;
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

        private static ConcurrentDictionary<string, string> PlayerConnectionDict = new ConcurrentDictionary<string, string>(); //client, connection

        private static ConcurrentDictionary<string, string> ConnectionPlayerDict
        {
            get
            {
                ConcurrentDictionary<string, string> reversedDict = new ConcurrentDictionary<string, string>();
                foreach (var kvp in PlayerConnectionDict)
                {
                    reversedDict[kvp.Value] = kvp.Key;
                }
                return reversedDict;
            }
        }


        [Authorize]
        public override async Task OnConnectedAsync()
        {
            string connectionId = Context.ConnectionId;
            string playerId = GetPlayerIdFromToken();
            PlayerConnectionDict.TryAdd(playerId, connectionId);
            await base.OnConnectedAsync();
            await SendMessageAsync($"user: {playerId}", "You connected to the server hub");
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            string connectionId = Context.ConnectionId;

            var keysToRemove = PlayerConnectionDict.Where(kvp => kvp.Value.Equals(connectionId)).Select(kvp => kvp.Key).ToList();

            foreach (var key in keysToRemove)
            {
                PlayerConnectionDict.TryRemove(connectionId, out _);
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

            var game = _matchmakingService.JoinQueue(playerId);

            if(game != null)
            {
                var players = game.Players.Select(x => x.PlayerId);

                foreach(var player in players)
                {
                    var currentPlayer = game.GetCurrentPlayer().PlayerId;
                    if (player == game.GetCurrentPlayer().PlayerId)
                    {
                        await Clients.Client(PlayerConnectionDict[player]).SendAsync("ReceiveMessage", playerId, "Found the game. It's your move", Trigger.FoundGamePlayerStarts, null);
                    }
                    else
                    {
                        await Clients.Client(PlayerConnectionDict[player]).SendAsync("ReceiveMessage", playerId, "Found the game. It's opponent's move", Trigger.FoundGameOpponentStarts, null);
                    }
                }
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
        public async Task PutInsect(InsectType type, (int, int, int)? whereToPut)
        {
            var playerId = GetPlayerIdFromToken();

            var request = new PutInsectRequest()
            {
                InsectToPut = type,
                PlayerId = playerId,
                WhereToPut = whereToPut
            };

            var result = _gameService.Put(request);
            var game = result.Game;

            var players = game.Players.Select(x => x.PlayerId);

            foreach (var player in players)
            {
                if (playerId == game.GetCurrentPlayer().PlayerId)
                {
                    await Clients.Client(PlayerConnectionDict[player]).SendAsync("ReceiveMessage", playerId, "Found the game. It's your move", Trigger.FoundGamePlayerStarts, result.CurrentBoard);
                }
                else
                {
                    await Clients.Client(PlayerConnectionDict[player]).SendAsync("ReceiveMessage", playerId, "Found the game. It's opponent's move", Trigger.FoundGameOpponentStarts, result.CurrentBoard);
                }
            }
        }

        [Authorize]
        public async Task MoveInsect(InsectType type)
        {
            throw new NotImplementedException();
        }

        private string GetPlayerIdFromToken()
        {
            var playerIdClaim = Context.User?.FindFirst("PlayerId");
            return playerIdClaim?.Value;
        }
    }
}