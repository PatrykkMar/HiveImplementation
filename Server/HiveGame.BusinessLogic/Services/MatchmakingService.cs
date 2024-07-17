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
using HiveGame.BusinessLogic.Repositories;

namespace HiveGame.BusinessLogic.Services
{
    public interface IMatchmakingService
    {
        string[]? JoinQueue(string clientId);
    }

    public class MatchmakingService : IMatchmakingService
    {
        private readonly IMatchmakingRepository _matchmakingRepository;
        private readonly IGameRepository _gameRepository;
        private readonly IGameFactory _gameFactory;
        public MatchmakingService(IMatchmakingRepository matchmakingRepository, IGameRepository gameRepository, IGameFactory gameFactory)
        {
            _matchmakingRepository = matchmakingRepository;
            _gameRepository = gameRepository;
            _gameFactory = gameFactory;
        }

        public string[]? JoinQueue(string clientId)
        {
            _matchmakingRepository.Add(new Player { PlayerId = clientId });

            if (_matchmakingRepository.Count >= 2)
            {
                var players = _matchmakingRepository.GetAndRemoveFirstTwo().ToArray();
                var game = _gameFactory.CreateGame(players);
                _gameRepository.Add(game);
                return players.Select(x => x.PlayerId).ToArray();
            }

            return null;
        }
    }
}