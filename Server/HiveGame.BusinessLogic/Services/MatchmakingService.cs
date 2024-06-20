using AutoMapper;
using HiveGame.BusinessLogic.Managers;
using HiveGame.BusinessLogic.Models.Game;
using HiveGame.BusinessLogic.Models;
using HiveGame.BusinessLogic.Models.Insects;
using HiveGame.BusinessLogic.Models.Requests;
using System.Net.WebSockets;
using HiveGame.BusinessLogic.Models.WebSocketModels;

namespace HiveGame.BusinessLogic.Services
{
    public interface IMatchmakingService
    {
        Task AddPlayerToQueueAsync(string playerName, string playerIP, WebSocket webSocket);
    }

    public class MatchmakingService : IMatchmakingService
    {
        long _nextGameId = 0;
        private readonly IPlayerManager _playerManager;
        private readonly IGameManager _gameManager;
        private readonly Queue<Player> _queue = new Queue<Player>();
        public MatchmakingService(IPlayerManager playerManager)
        {
            _playerManager = playerManager;
        }

        public async Task AddPlayerToQueueAsync(string playerName, string playerIP, WebSocket webSocket)
        {
            var player = new Player { Nick = playerName, IP = playerIP, WebSocket = webSocket };
            _queue.Enqueue(player);
            _playerManager.AddClient(playerName, playerIP, webSocket);

            if (_queue.Count >= 2)
            {
                var players = new List<Player>
                {
                    _queue.Dequeue(),
                    _queue.Dequeue()
                };

                var game = new Game();
                _gameManager.AddGame(game);

                var message = new WebSocketMessage
                {
                    Message = "Utworzono grę",
                    Type = MessageType.GameCreated
                };

                foreach (var p in players)
                {
                    await _playerManager.SendMessageAsync(p.Nick, message);
                }
            }
        }
    }
}