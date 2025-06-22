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
using HiveGame.Core.Models;

namespace HiveGame.BusinessLogic.Services
{

    public interface IMatchmakingService
    {
        Game? JoinQueue(string clientId, string playerNick);
        void LeaveQueue(string clientId);
        Player CreatePlayer();
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
            var player = _matchmakingRepository.GetByPlayerId(clientId);
            player.PlayerNick = playerNick;

            _matchmakingRepository.Update(clientId, player);

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
            var player = _matchmakingRepository.GetByPlayerId(clientId);
            player.PlayerState = ClientState.Connected;
            _matchmakingRepository.Update(clientId, player);
        }

        public Player CreatePlayer()
        {
            var player = new Player() { PlayerId = Guid.NewGuid().ToString(), PlayerState = ClientState.Connected };
            _matchmakingRepository.Add(player);
            return player;
        }
    }
}