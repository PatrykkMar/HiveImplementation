using AutoMapper;
using HiveGame.BusinessLogic.Models;
using HiveGame.BusinessLogic.Models;
using HiveGame.BusinessLogic.Models.Insects;
using HiveGame.BusinessLogic.Models.Requests;
using System.Net.WebSockets;
using HiveGame.BusinessLogic.Models.WebSocketModels;
using HiveGame.BusinessLogic.Factories;
using HiveGame.BusinessLogic.Models.Board;
using HiveGame.BusinessLogic.Repositories;
using HiveGame.BusinessLogic.Utils;
using HiveGame.DataAccess.Repositories;

namespace HiveGame.BusinessLogic.Services
{

    public interface IMatchmakingService
    {
        Game? JoinQueue(string clientId, string playerNick);
        void LeaveQueue(string clientId);
    }

    public class MatchmakingService : IMatchmakingService
    {
        private const int PLAYERS_TO_START_GAME = 2;

        private readonly IMatchmakingRepository _matchmakingRepository;
        private readonly IGameRepository _gameRepository;
        private readonly IGameConverter _converter;
        private readonly IGameFactory _gameFactory;
        public MatchmakingService(IMatchmakingRepository matchmakingRepository, IGameRepository gameRepository, IGameFactory gameFactory, IGameConverter converter)
        {
            _matchmakingRepository = matchmakingRepository;
            _gameRepository = gameRepository;
            _gameFactory = gameFactory;
            _converter = converter;
        }

        public Game? JoinQueue(string clientId, string playerNick)
        {
            if(_matchmakingRepository.GetByPlayerId(clientId) == null)
                _matchmakingRepository.Add(new Player { PlayerId = clientId, PlayerNick = playerNick });

            if (_matchmakingRepository.Count >= PLAYERS_TO_START_GAME)
            {
                var players = _matchmakingRepository.GetAndRemoveFirstTwo().ToArray();
                var game = _gameFactory.CreateGame(players);
                _gameRepository.Add(_converter.ToGameDbModel(game));
                return game;
            }

            return null;
        }

        public void LeaveQueue(string clientId)
        {
            _matchmakingRepository.Remove(clientId);
        }
    }
}