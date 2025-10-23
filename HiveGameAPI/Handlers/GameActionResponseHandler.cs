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
using Microsoft.AspNetCore.Mvc.Diagnostics;
using HiveGame.Hubs;

namespace HiveGame.Handlers
{

    public interface IGameActionsResponseHandler
    {
        Task PutFirstInsectAsync(InsectType type, string playerId);
        Task PutInsectAsync(InsectType type, Point2D position, string playerId);
        Task MoveInsectAsync(Point2D moveFrom, Point2D moveTo, string playerId);
        Task JoinQueueAsync(string playerId, string playerNick);
        Task LeaveQueueAsync(string playerId);
        Task OnPlayerDisconnectedFromGameAsync(string playerId);
    }
    public class GameActionsResponseHandler : IGameActionsResponseHandler
    {
        private readonly IHiveGameService _gameService;
        private readonly IConnectionManager _connectionManager;
        private readonly IMatchmakingService _matchmakingService;
        private readonly IHubContext<GameHub> _hubContext;

        public GameActionsResponseHandler(IHiveGameService gameService, IConnectionManager connectionManager, IMatchmakingService matchmakingService, IHubContext<GameHub> hubContext)
        {
            _gameService = gameService;
            _connectionManager = connectionManager;
            _matchmakingService = matchmakingService;
            _hubContext = hubContext;
        }

        public void BeforeMoveAction(string playerId)
        {
            _connectionManager.RemoveDisconnectPlayerToken(playerId);
        }

        //queue actions
        public async Task JoinQueueAsync(string playerId, string playerNick)
        {

            var result = await _matchmakingService.JoinQueueAsync(playerId, playerNick);

            if (result.Game != null)
            {
                foreach (var playerInGame in result.Game.Players)
                    await SendPlayerStateAndViewAsync(playerInGame, additionalMessage: $"Your opponent is {result.Game.GetOtherPlayer(playerInGame.PlayerId)}", playerView: result.Game.GetPlayerView(playerInGame.PlayerId));
            }
            else if (result.Player != null)
                await SendPlayerStateAndViewAsync(result.Player);
            else
                throw new InvalidOperationException("Neither game nor player are returned");
        }

        public async Task LeaveQueueAsync(string playerId)
        {
            var result = _matchmakingService.LeaveQueue(playerId);
            await SendPlayerStateAndViewAsync(result.Player);
        }

        //game actions
        public async Task PutFirstInsectAsync(InsectType type, string playerId)
        {
            BeforeMoveAction(playerId);
            var request = new PutFirstInsectRequest
            {
                InsectToPut = type,
                PlayerId = playerId
            };

            var result = await _gameService.PutFirstInsectAsync(request);
            foreach (var playerInGame in result.Game.Players)
                await SendPlayerStateAndViewAsync(playerInGame, playerView: result.Game.GetPlayerView(playerInGame.PlayerId));
        }

        public async Task PutInsectAsync(InsectType type, Point2D position, string playerId)
        {
            BeforeMoveAction(playerId);
            var request = new PutInsectRequest
            {
                InsectToPut = type,
                PlayerId = playerId,
                WhereToPut = position
            };

            var result = await _gameService.PutAsync(request);
            foreach (var playerInGame in result.Game.Players)
                await SendPlayerStateAndViewAsync(playerInGame, additionalMessage: $"Your opponent is {result.Game.GetOtherPlayer(playerInGame.PlayerId)}", playerView: result.Game.GetPlayerView(playerInGame.PlayerId));
        }

        public async Task MoveInsectAsync(Point2D moveFrom, Point2D moveTo, string playerId)
        {
            BeforeMoveAction(playerId);
            var request = new MoveInsectRequest
            {
                MoveFrom = moveFrom,
                MoveTo = moveTo,
                PlayerId = playerId
            };

            var result = await _gameService.MoveAsync(request);
            foreach (var playerInGame in result.Game.Players)
                await SendPlayerStateAndViewAsync(playerInGame, additionalMessage: $"Your opponent is {result.Game.GetOtherPlayer(playerInGame.PlayerId)}", playerView: result.Game.GetPlayerView(playerInGame.PlayerId));
        }
        public async Task OnPlayerDisconnectedFromGameAsync(string playerId)
        {
            var result = await _gameService.SetDisconnectedFromGamePlayerWarningAsync(playerId);

            //warning
            await SendPlayerStateAndViewAsync(result.Game.GetOtherPlayer(playerId), additionalMessage: $"Your opponent disconnected. If he doesn't reconnect in 15 seconds, the game will end", withoutState: true);

            //set timer for 15 seconds to disconnect. After it make metod _matchmakingService.EndGame()

            var cts = _connectionManager.AddDisconnectPlayerToken(playerId);

            _ = Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(15), cts.Token);
                    await _gameService.EndGameAsync(result.Game);
                }
                catch (TaskCanceledException)
                {
                    // Player is back
                    await SendPlayerStateAndViewAsync(result.Game.GetOtherPlayer(playerId), additionalMessage: $"Player is back", withoutState: true);
                }
                finally
                {
                    _connectionManager.RemoveDisconnectPlayerToken(playerId);
                }
            });
        }

        private async Task SendPlayerStateAndViewAsync(Player player, bool withoutState = false, string additionalMessage = "", PlayerViewDTO playerView = null)
        {
            var connectionId = _connectionManager.GetConnectionId(player.PlayerId);
            if (connectionId != null)
                await _hubContext.Clients.Client(connectionId).SendAsync("ReceiveMessage", new ReceiveMessageRequest(player.PlayerId, string.Empty, withoutState ? null : player.PlayerState, playerView));
        }
    }
}
