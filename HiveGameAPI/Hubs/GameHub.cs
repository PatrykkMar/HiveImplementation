using HiveGame.BusinessLogic.Models;
using HiveGame.BusinessLogic.Models.Insects;
using HiveGame.BusinessLogic.Utils;
using HiveGame.Core.Models;
using HiveGame.Handlers;
using HiveGame.Managers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace HiveGame.Hubs
{
    public class GameHub : Hub
    {
        private readonly IConnectionManager _connectionManager;
        private readonly IGameActionsHandler _gameActionsHandler;

        public GameHub(IConnectionManager connectionManager, IGameActionsHandler gameActionsHandler)
        {
            _connectionManager = connectionManager;
            _gameActionsHandler = gameActionsHandler;
        }

        [Authorize(Roles = Roles.Player)]
        public override async Task OnConnectedAsync()
        {
            string connectionId = Context.ConnectionId;
            string playerId = GetPlayerIdFromToken();
            _connectionManager.AddPlayerConnection(playerId, connectionId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _connectionManager.RemovePlayerConnection(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        [Authorize(Roles = Roles.Player)]
        public async Task JoinQueue()
        {
            var playerId = GetPlayerIdFromToken();
            await _gameActionsHandler.JoinQueue(playerId, Clients);
        }

        [Authorize(Roles = Roles.Player)]
        public async Task LeaveQueue()
        {

            var playerId = GetPlayerIdFromToken();
            await _gameActionsHandler.LeaveQueue(playerId, Clients);
        }

        [Authorize(Roles = Roles.Player)]
        public async Task PutFirstInsect(InsectType type)
        {
            var playerId = GetPlayerIdFromToken();
            await _gameActionsHandler.PutFirstInsectAsync(type, playerId, Clients);
        }

        [Authorize(Roles = Roles.Player)]
        public async Task PutInsect(InsectType type, int[] whereToPut)
        {
            var playerId = GetPlayerIdFromToken();
            var position = new Point2D(whereToPut[0], whereToPut[1]);
            await _gameActionsHandler.PutInsectAsync(type, position, playerId, Clients);
        }

        [Authorize(Roles = Roles.Player)]
        public async Task MoveInsect(int[] moveFrom, int[] moveTo)
        {
            var playerId = GetPlayerIdFromToken();
            var fromPosition = new Point2D(moveFrom[0], moveFrom[1]);
            var toPosition = new Point2D(moveTo[0], moveTo[1]);
            await _gameActionsHandler.MoveInsectAsync(fromPosition, toPosition, playerId, Clients);
        }

        [Authorize(Roles = Roles.Player)]
        public async Task FinishGame()
        {
            
        }


        private string GetPlayerIdFromToken()
        {
            var playerIdClaim = Context.User?.FindFirst("PlayerId");
            return playerIdClaim?.Value;
        }
    }
}