using HiveGame.BusinessLogic.Models;
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
using System.Numerics;

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
                    PlayerViewDTO playerView = game.GetPlayerView(player);
                    var currentPlayer = game.GetCurrentPlayer().PlayerId;
                    var otherPlayer = game.GetOtherPlayer().PlayerId;
                    if (player == currentPlayer)
                    {
                        await Clients.Client(PlayerConnectionDict[player]).SendAsync("ReceiveMessage", playerId, "Found the game. It's your move", Trigger.FoundGamePlayerStarts, playerView);
                    }
                    else if (player == otherPlayer)
                    {
                        await Clients.Client(PlayerConnectionDict[player]).SendAsync("ReceiveMessage", playerId, "Found the game. It's opponent's move", Trigger.FoundGameOpponentStarts, playerView);
                    }
                    else
                    {
                        throw new Exception("Message sent to a player which is not a participant of a game");
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
        public async Task PutFirstInsect(InsectType type)
        {
            var playerId = GetPlayerIdFromToken();

            var request = new PutFirstInsectRequest()
            {
                InsectToPut = type,
                PlayerId = playerId
            };

            var result = _gameService.PutFirstInsect(request);
            await SendGameActionInformation(result);
        }

        [Authorize]
        public async Task PutInsect(InsectType type, int[] whereToPut)
        {
            var playerId = GetPlayerIdFromToken();

            var request = new PutInsectRequest()
            {
                InsectToPut = type,
                PlayerId = playerId,
                WhereToPut = (whereToPut[0], whereToPut[1])
            };

            var result = _gameService.Put(request);
            await SendGameActionInformation(result);
        }

        [Authorize]
        public async Task MoveInsect(int[] moveFrom, int[] moveTo)
        {
            var playerId = GetPlayerIdFromToken();

            var request = new MoveInsectRequest()
            {
                MoveFrom = (moveFrom[0], moveFrom[1]),
                MoveTo= (moveTo[0], moveTo[1]),
                PlayerId = playerId
            };

            var result = _gameService.Move(request);
            await SendGameActionInformation(result);
        }

        private string GetPlayerIdFromToken()
        {
            var playerIdClaim = Context.User?.FindFirst("PlayerId");
            return playerIdClaim?.Value;
        }

        private async Task SendGameActionInformation(HiveActionResult result)
        {
            var game = result.Game;

            var players = game.Players.Select(x => x.PlayerId);
            var playerId = GetPlayerIdFromToken();
            var gameOver = result.GameOver;

            foreach (var player in players)
            {
                PlayerViewDTO playerView = game.GetPlayerView(player);

                if (gameOver)
                {
                    await Clients.Client(PlayerConnectionDict[player]).SendAsync("ReceiveMessage", playerId, "Game is over", Trigger.EndGameConditionMet, playerView);
                    continue;
                }

                if (player == game.GetCurrentPlayer().PlayerId)
                {
                    if(game.Board.FirstMoves)
                        await Clients.Client(PlayerConnectionDict[player]).SendAsync("ReceiveMessage", playerId, "It's your first move", Trigger.PlayerFirstMove, playerView);
                    else
                        await Clients.Client(PlayerConnectionDict[player]).SendAsync("ReceiveMessage", playerId, "It's your move", Trigger.PlayerMove, playerView);
                }
                else
                {
                    await Clients.Client(PlayerConnectionDict[player]).SendAsync("ReceiveMessage", playerId, "It's opponent's move", Trigger.PlayerMadeMove, playerView);
                }
            }
        }
    }
}