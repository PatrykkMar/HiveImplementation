using HiveGame.BusinessLogic.Models.Insects;
using HiveGame.BusinessLogic.Models.Requests;
using HiveGame.BusinessLogic.Models;
using Microsoft.AspNetCore.Authorization;
using HiveGame.BusinessLogic.Services;
using HiveGame.Managers;
using Microsoft.AspNetCore.SignalR;
using HiveGame.Core.Models;
using HiveGame.Core.Models.Requests;

namespace HiveGame.Handlers
{

    public interface IGameActionsHandler
    {
        Task PutFirstInsectAsync(InsectType type, string playerId, IHubCallerClients clients);
        Task PutInsectAsync(InsectType type, Point2D position, string playerId, IHubCallerClients clients);
        Task MoveInsectAsync(Point2D moveFrom, Point2D moveTo, string playerId, IHubCallerClients clients);
        Task JoinQueue(string playerId, IHubCallerClients clients);
        Task LeaveQueue(string playerId, IHubCallerClients clients);
    }
    public class GameActionsHandler : IGameActionsHandler
    {
        private readonly IHiveGameService _gameService;
        private readonly IConnectionManager _connectionManager;
        private readonly IMatchmakingService _matchmakingService;

        public GameActionsHandler(IHiveGameService gameService, IConnectionManager connectionManager, IMatchmakingService matchmakingService)
        {
            _gameService = gameService;
            _connectionManager = connectionManager;
            _matchmakingService = matchmakingService;
        }

        public async Task PutFirstInsectAsync(InsectType type, string playerId, IHubCallerClients clients)
        {
            var request = new PutFirstInsectRequest
            {
                InsectToPut = type,
                PlayerId = playerId
            };

            var result = _gameService.PutFirstInsect(request);
            await NotifyGameActionResultAsync(result, playerId, clients);
        }

        public async Task PutInsectAsync(InsectType type, Point2D position, string playerId, IHubCallerClients clients)
        {
            var request = new PutInsectRequest
            {
                InsectToPut = type,
                PlayerId = playerId,
                WhereToPut = position
            };

            var result = _gameService.Put(request);
            await NotifyGameActionResultAsync(result, playerId, clients);
        }

        public async Task MoveInsectAsync(Point2D moveFrom, Point2D moveTo, string playerId, IHubCallerClients clients)
        {
            var request = new MoveInsectRequest
            {
                MoveFrom = moveFrom,
                MoveTo = moveTo,
                PlayerId = playerId
            };

            var result = _gameService.Move(request);
            await NotifyGameActionResultAsync(result, playerId, clients);
        }

        [Authorize]
        public async Task JoinQueue(string playerId, IHubCallerClients clients)
        {

            var game = _matchmakingService.JoinQueue(playerId);

            if (game != null)
            {
                var players = game.Players.Select(x => x.PlayerId);

                foreach (var player in players)
                {
                    PlayerViewDTO playerView = game.GetPlayerView(player);
                    var currentPlayer = game.GetCurrentPlayer().PlayerId;
                    var otherPlayer = game.GetOtherPlayer().PlayerId;

                    var connection = _connectionManager.GetConnectionId(player);

                    if (connection == null)
                        throw new Exception("Connection not found");

                    if (player == currentPlayer)
                    {
                        await clients.Client(connection)
                            .SendAsync("ReceiveMessage", new ReceiveMessageRequest(playerId, "Found the game. It's your move", ClientState.InGamePlayerFirstMove, playerView));
                    }
                    else if (player == otherPlayer)
                    {
                        await clients.Client(connection)
                            .SendAsync("ReceiveMessage", new ReceiveMessageRequest(playerId, "Found the game. It's opponent's move", ClientState.InGameOpponentMove, playerView));
                    }
                    else
                    {
                        throw new Exception("Message sent to a player which is not a participant of a game");
                    }
                }
            }
            else
            {
                await clients.Caller.SendAsync("ReceiveMessage", new ReceiveMessageRequest(playerId, "Waiting for player", ClientState.WaitingForPlayers, null));
            }
        }

        [Authorize]
        public async Task LeaveQueue(string playerId, IHubCallerClients clients)
        {

            _matchmakingService.LeaveQueue(playerId);

            await clients.Caller.SendAsync("ReceiveMessage", new ReceiveMessageRequest(playerId, "Left the queue", ClientState.Connected, null));
        }

        private async Task NotifyGameActionResultAsync(HiveActionResult result, string playerId, IHubCallerClients clients)
        {
            var game = result.Game;
            var isGameOver = result.GameOver;

            foreach (var player in game.Players)
            {
                var playerView = game.GetPlayerView(player.PlayerId);
                var connectionId = _connectionManager.GetConnectionId(player.PlayerId);
                var isCurrentPlayer = player.PlayerId == game.GetCurrentPlayer().PlayerId;

                if (connectionId != null)
                {
                    if (isGameOver)
                    {
                        await clients.Client(connectionId).SendAsync("ReceiveMessage", new ReceiveMessageRequest(playerId, "Game is over", ClientState.GameOver, playerView));
                    }
                    else if (isCurrentPlayer)
                    {
                        var message = game.Board.FirstMoves ? "It's your first move" : "It's your move";
                        var state = game.Board.FirstMoves ? ClientState.InGamePlayerFirstMove : ClientState.InGamePlayerMove;
                        await clients.Client(connectionId).SendAsync("ReceiveMessage", new ReceiveMessageRequest(playerId, message, state, playerView));
                    }
                    else
                    {
                        await clients.Client(connectionId).SendAsync("ReceiveMessage", new ReceiveMessageRequest(playerId, "It's opponent's move", ClientState.InGameOpponentMove, playerView));
                    }
                }
            }
        }
    }
}
