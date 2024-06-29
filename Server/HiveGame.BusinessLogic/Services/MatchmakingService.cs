using AutoMapper;
using HiveGame.BusinessLogic.Managers;
using HiveGame.BusinessLogic.Models.Game;
using HiveGame.BusinessLogic.Models;
using HiveGame.BusinessLogic.Models.Insects;
using HiveGame.BusinessLogic.Models.Requests;
using System.Net.WebSockets;
using HiveGame.BusinessLogic.Models.WebSocketModels;
using HiveGame.BusinessLogic.Factories;
using HiveGame.BusinessLogic.Models.Game.Graph;

namespace HiveGame.BusinessLogic.Services
{
    public interface IMatchmakingService
    {
        Task JoinQueue(string nick, WebSocket webSocket);
    }

    public class MatchmakingService : IMatchmakingService
    {
        long _nextGameId = 0;
        private readonly IPlayerConnectionManager _playerManager;
        private readonly IGameManager _gameManager;
        private readonly IInsectFactory _insectFactory;
        private readonly Queue<Player> _queue = new Queue<Player>();
        public MatchmakingService(IPlayerConnectionManager playerManager, IInsectFactory factory, IGameManager gameManager)
        {
            _playerManager = playerManager;
            _insectFactory = factory;
            _gameManager = gameManager;
        }

        public async Task JoinQueue(string nick, WebSocket webSocket)
        {
            throw new NotImplementedException();
        }
    }
}