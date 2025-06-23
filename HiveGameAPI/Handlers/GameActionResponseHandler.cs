using HiveGame.BusinessLogic.Models.Insects;
using HiveGame.BusinessLogic.Models.Requests;
using HiveGame.BusinessLogic.Models;
using Microsoft.AspNetCore.Authorization;
using HiveGame.BusinessLogic.Services;
using HiveGame.Managers;
using Microsoft.AspNetCore.SignalR;
using HiveGame.Core.Models;
using HiveGame.Core.Models.Requests;
using MongoDB.Driver.Core.Connections;
using System.Numerics;

namespace HiveGame.Handlers
{

    public interface IGameActionsResponseHandler
    {
        Task PutFirstInsectAsync(InsectType type, string playerId, IHubCallerClients clients);
        Task PutInsectAsync(InsectType type, Point2D position, string playerId, IHubCallerClients clients);
        Task MoveInsectAsync(Point2D moveFrom, Point2D moveTo, string playerId, IHubCallerClients clients);
        Task JoinQueue(string playerId, string playerNick, IHubCallerClients clients);
        Task LeaveQueue(string playerId, IHubCallerClients clients);
    }
    public class GameActionsResponseHandler : IGameActionsResponseHandler
    {
        private readonly IHiveGameService _gameService;
        private readonly IConnectionManager _connectionManager;
        private readonly IMatchmakingService _matchmakingService;

        public GameActionsResponseHandler(IHiveGameService gameService, IConnectionManager connectionManager, IMatchmakingService matchmakingService)
        {
            _gameService = gameService;
            _connectionManager = connectionManager;
            _matchmakingService = matchmakingService;
        }

        //queue actions
        public async Task JoinQueue(string playerId, string playerNick, IHubCallerClients clients)
        {

            var result = _matchmakingService.JoinQueue(playerId, playerNick);

            if (result.Game != null)
            {
                foreach (var playerInGame in result.Game.Players)
                    await SendPlayerStateAndViewAsync(clients, playerInGame, additionalMessage: $"Your opponent is {result.Game.GetOtherPlayer(playerInGame.PlayerId)}", playerView: result.Game.GetPlayerView(playerInGame.PlayerId));
            }
            else if (result.Player != null)
                await SendPlayerStateAndViewAsync(clients, result.Player);
            else 
                throw new InvalidOperationException("Neither game nor player are returned");
        }

        public async Task LeaveQueue(string playerId, IHubCallerClients clients)
        {
            var result = _matchmakingService.LeaveQueue(playerId);
            await SendPlayerStateAndViewAsync(clients, result.Player);
        }

        //game actions
        public async Task PutFirstInsectAsync(InsectType type, string playerId, IHubCallerClients clients)
        {
            var request = new PutFirstInsectRequest
            {
                InsectToPut = type,
                PlayerId = playerId
            };

            var result = _gameService.PutFirstInsect(request);
            foreach (var playerInGame in result.Game.Players)
                await SendPlayerStateAndViewAsync(clients, playerInGame, playerView: result.Game.GetPlayerView(playerInGame.PlayerId));
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
            foreach (var playerInGame in result.Game.Players)
                await SendPlayerStateAndViewAsync(clients, playerInGame, additionalMessage: $"Your opponent is {result.Game.GetOtherPlayer(playerInGame.PlayerId)}", playerView: result.Game.GetPlayerView(playerInGame.PlayerId));
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
            foreach (var playerInGame in result.Game.Players)
                await SendPlayerStateAndViewAsync(clients, playerInGame, additionalMessage: $"Your opponent is {result.Game.GetOtherPlayer(playerInGame.PlayerId)}", playerView: result.Game.GetPlayerView(playerInGame.PlayerId));
        }

        private async Task SendPlayerStateAndViewAsync(IHubCallerClients clients, Player player, string additionalMessage = "", PlayerViewDTO playerView = null)
        {
            var connectionId = _connectionManager.GetConnectionId(player.PlayerId);
            if (connectionId != null)
                await clients.Client(connectionId).SendAsync("ReceiveMessage", new ReceiveMessageRequest(player.PlayerId, string.Empty, player.PlayerState, playerView));
        }
    }
}
