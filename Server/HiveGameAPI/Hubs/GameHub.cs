using HiveGame.BusinessLogic.Models;
using HiveGame.BusinessLogic.Models.Insects;
using HiveGame.BusinessLogic.Models.Requests;
using HiveGame.BusinessLogic.Services;
using HiveGame.BusinessLogic.Utils;
using HiveGame.Managers;
using HiveGame.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace HiveGame.Hubs
{
    public class GameHub : Hub
    {
        private readonly ITokenUtils _utils;
        private readonly IMatchmakingService _matchmakingService;
        private readonly IHiveGameService _gameService;
        private readonly IConnectionManager _connectionManager;

        public GameHub(ITokenUtils utils, IMatchmakingService matchmakingService, IHiveGameService gameService, IConnectionManager connectionManager)
        {
            _utils = utils;
            _matchmakingService = matchmakingService;
            _gameService = gameService;
            _connectionManager = connectionManager;
        }

        [Authorize]
        public override async Task OnConnectedAsync()
        {
            string connectionId = Context.ConnectionId;
            string playerId = GetPlayerIdFromToken();
            _connectionManager.AddPlayerConnection(playerId, connectionId);
            await base.OnConnectedAsync();
            await SendMessageAsync($"user: {playerId}", "You connected to the server hub");
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _connectionManager.RemovePlayerConnection(Context.ConnectionId);
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
                    var connectionId = _connectionManager.GetConnectionId(player);

                    if (string.IsNullOrEmpty(connectionId))
                        throw new Exception("Connection not found");

                    if (string.IsNullOrEmpty(connectionId))
                        throw new Exception("Connection not found");

                    if (player == currentPlayer)
                    {
                        await Clients.Client(connectionId).SendAsync("ReceiveMessage", playerId, "Found the game. It's your move", ClientState.InGamePlayerFirstMove, playerView);
                    }
                    else if (player == otherPlayer)
                    {
                        await Clients.Client(connectionId).SendAsync("ReceiveMessage", playerId, "Found the game. It's opponent's move", ClientState.InGameOpponentMove, playerView);
                    }
                    else
                    {
                        throw new Exception("Message sent to a player which is not a participant of a game");
                    }
                }
            }
            else
            {
                await Clients.Caller.SendAsync("ReceiveMessage", playerId, "Waiting for player", ClientState.WaitingForPlayers, null);
            }
        }

        [Authorize]
        public async Task LeaveQueue()
        {

            var playerId = GetPlayerIdFromToken();

            _matchmakingService.LeaveQueue(playerId);

            await Clients.Caller.SendAsync("ReceiveMessage", playerId, "Left the queue", ClientState.Connected, null);
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

                var connectionId = _connectionManager.GetConnectionId(player);

                if (string.IsNullOrEmpty(connectionId))
                    throw new Exception("Connection not found");

                if (gameOver)
                {
                    await Clients.Client(connectionId).SendAsync("ReceiveMessage", playerId, "Game is over", ClientState.GameOver, playerView);
                    continue;
                }

                if (player == game.GetCurrentPlayer().PlayerId)
                {
                    if(game.Board.FirstMoves)
                        await Clients.Client(connectionId).SendAsync("ReceiveMessage", playerId, "It's your first move", ClientState.InGamePlayerFirstMove, playerView);
                    else
                        await Clients.Client(connectionId).SendAsync("ReceiveMessage", playerId, "It's your move", ClientState.InGamePlayerMove, playerView);
                }
                else
                {
                    await Clients.Client(connectionId).SendAsync("ReceiveMessage", playerId, "It's opponent's move", ClientState.InGameOpponentMove, playerView);
                }
            }
        }
    }
}