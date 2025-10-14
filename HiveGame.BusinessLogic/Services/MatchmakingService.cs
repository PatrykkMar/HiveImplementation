using HiveGame.BusinessLogic.Models;
using HiveGame.BusinessLogic.Factories;
using HiveGame.BusinessLogic.Repositories;
using HiveGame.BusinessLogic.Utils;
using HiveGame.DataAccess.Repositories;
using HiveGame.Core.Models;

namespace HiveGame.BusinessLogic.Services
{

    public interface IMatchmakingService
    {
        Task<JoinQueueResult> JoinQueueAsync(string clientId, string playerNick);
        LeaveQueueResult LeaveQueue(string clientId);
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

        public async Task<JoinQueueResult> JoinQueueAsync(string clientId, string playerNick)
        {
            var player = _matchmakingRepository.GetByPlayerId(clientId);
            player.PlayerNick = playerNick;
            player.PlayerState = ClientState.WaitingForPlayers;

            _matchmakingRepository.Update(clientId, player);
            var countInQueue = _matchmakingRepository.CountInQueue;

            if (countInQueue >= PLAYERS_TO_START_GAME)
            {
                var players = _matchmakingRepository.GetAndRemoveFirstTwoInQueue().ToArray();
                var game = _gameFactory.CreateGame(players);
                players[0].PlayerState = ClientState.InGamePlayerFirstMove;
                players[1].PlayerState = ClientState.InGameOpponentMove;
                foreach(var playerInGame in players)
                    _matchmakingRepository.Update(playerInGame.PlayerId, playerInGame);

                var result = new JoinQueueResult { Game = game };
                await _gameRepository.AddAsync(_converter.ToGameDbModel(game));
                return result;
            }

            return new JoinQueueResult { Player = player };
        }

        public LeaveQueueResult LeaveQueue(string clientId)
        {
            var player = _matchmakingRepository.GetByPlayerId(clientId);
            player.PlayerState = ClientState.Connected;
            _matchmakingRepository.Update(clientId, player);
            return new LeaveQueueResult { Player = player };
        }

        public Player CreatePlayer()
        {
            var player = new Player() { PlayerId = Guid.NewGuid().ToString(), PlayerState = ClientState.Connected };
            _matchmakingRepository.Add(player);
            return player;
        }
    }
}