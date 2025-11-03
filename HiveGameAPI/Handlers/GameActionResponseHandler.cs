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
using HiveGame.Utils;

namespace HiveGame.Handlers
{

    public interface IGameActionsResponseHandler
    {
        Task PutFirstInsectAsync(InsectType type, string playerId);
        Task PutInsectAsync(InsectType type, Point2D position, string playerId);
        Task MoveInsectAsync(Point2D moveFrom, Point2D moveTo, string playerId);
        Task JoinQueueAsync(string playerId, string playerNick);
        Task LeaveQueueAsync(string playerId);
        Task FinishGameAsync(string playerId);
        Task ConfirmGameAsync(string playerId);
        Task HandlePendingMatchTimeoutAsync(PendingPlayers players);
        Task OnPlayerDisconnectedFromGameAsync(string playerId);
    }
    public class GameActionsResponseHandler : IGameActionsResponseHandler
    {
        private readonly IHiveGameService _gameService;
        private readonly IConnectionManager _connectionManager;
        private readonly IMatchmakingService _matchmakingService;
        private readonly IHubContext<GameHub> _hubContext;
        private readonly PendingPlayersWatcher _pendingPlayersWatcher;

        public GameActionsResponseHandler(IHiveGameService gameService, IConnectionManager connectionManager, IMatchmakingService matchmakingService, IHubContext<GameHub> hubContext, PendingPlayersWatcher pendingPlayersWatcher)
        {
            _gameService = gameService;
            _connectionManager = connectionManager;
            _matchmakingService = matchmakingService;
            _hubContext = hubContext;
            _pendingPlayersWatcher = pendingPlayersWatcher;

            _pendingPlayersWatcher.OnTimeout += HandlePendingMatchTimeoutAsync;
        }

        public async Task OnPlayerDisconnectedFromGameAsync(string playerId)
        {
            var result = await _matchmakingService.HandleDisconnectedPlayer(playerId);
            await _connectionManager.RemovePlayerConnectionAsync(playerId);
            if(result.OtherPlayer != null)
            {
                await SendPlayerStateAndViewAsync(result.OtherPlayer, additionalMessage: $"Your opponent disconnected");
            }
        }

        private async Task SendPlayerStateAndViewAsync(Player player, bool withoutState = false, string additionalMessage = "", PlayerViewDTO playerView = null)
        {
            var connectionId = _connectionManager.GetConnectionId(player.PlayerId);
            if (connectionId != null)
                await _hubContext.Clients.Client(connectionId).SendAsync("ReceiveMessage", new ReceiveMessageRequest(player.PlayerId, string.IsNullOrEmpty(additionalMessage) ? string.Empty : additionalMessage, withoutState ? null : player.PlayerState, playerView));
        }

        public async Task HandlePendingMatchTimeoutAsync(PendingPlayers players)
        {
            //set statuses
            var result = await _matchmakingService.HandlePendingTimeoutAsync(players);

            //send information about timeout.
            foreach(var player in result.PendingPlayers.Players)
            {
                if(player.PlayerState == ClientState.WaitingInQueue)
                {
                    //join queue operation again
                    await JoinQueueAsync(player.PlayerId, player.PlayerNick);
                }
                else if (player.PlayerState == ClientState.Connected)
                    await SendPlayerStateAndViewAsync(player);
                else
                    throw new InvalidOperationException("There shouldn't be another state in this method. Found state: " + Enum.GetName(player.PlayerState) ?? "no state");
            }
        }


        #region Queue actions
        public async Task JoinQueueAsync(string playerId, string playerNick)
        {

            var result = await _matchmakingService.JoinQueueAsync(playerId, playerNick);

            if (result.PendingPlayers != null)
            {
                foreach (var playerInGame in result.PendingPlayers.Players)
                    await SendPlayerStateAndViewAsync(playerInGame, additionalMessage: $"Your opponent is {result.PendingPlayers.GetOtherPlayer(playerInGame.PlayerId).PlayerNick}");

                _pendingPlayersWatcher.AddPendingPlayers(result.PendingPlayers);
                //setting 10 seconds for pending players to confirm
            }
            else if (result.Player != null)
                await SendPlayerStateAndViewAsync(result.Player);
            else
                throw new InvalidOperationException("Neither PendingPlayers nor player are returned");
        }

        public async Task LeaveQueueAsync(string playerId)
        {
            var result = _matchmakingService.LeaveQueue(playerId);
            await SendPlayerStateAndViewAsync(result.Player);
        }

        public async Task ConfirmGameAsync(string playerId)
        {
            var result = await _matchmakingService.ConfirmGameAsync(playerId);
            if (result.Game != null)
            {
                foreach (var playerInGame in result.Game.Players)
                    await SendPlayerStateAndViewAsync(playerInGame, additionalMessage: $"Your opponent is {result.Game.GetOtherPlayer(playerInGame.PlayerId)}", playerView: result.Game.GetPlayerView(playerInGame.PlayerId));
            }
            else if (result.Player != null)
                await SendPlayerStateAndViewAsync(result.Player);
            else
                throw new InvalidOperationException("Neither Game nor Player are returned");
        }

        #endregion

        #region Game actions

        public void BeforeMoveAction(string playerId)
        {
            _connectionManager.RemoveDisconnectPlayerToken(playerId);
        }

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
                await SendPlayerStateAndViewAsync(playerInGame, playerView: result.Game.GetPlayerView(playerInGame.PlayerId));
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
                await SendPlayerStateAndViewAsync(playerInGame, playerView: result.Game.GetPlayerView(playerInGame.PlayerId));
        }

        public async Task FinishGameAsync(string playerId)
        {
            var result = await _matchmakingService.FinishGameAsync(playerId);
            await SendPlayerStateAndViewAsync(result.Player);
        }

        #endregion
    }
}
