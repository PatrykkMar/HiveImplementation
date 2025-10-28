using HiveGame.BusinessLogic.Models;
using HiveGame.BusinessLogic.Factories;
using HiveGame.BusinessLogic.Repositories;
using HiveGame.BusinessLogic.Utils;
using HiveGame.DataAccess.Repositories;
using HiveGame.Core.Models;
using System.Runtime.CompilerServices;

namespace HiveGame.BusinessLogic.Services
{

    public interface IMatchmakingService
    {
        Task<JoinQueueResult> JoinQueueAsync(string clientId, string playerNick);
        LeaveQueueResult LeaveQueue(string clientId);
        Task<ConfirmGameResult> ConfirmGameAsync(string clientId);
        Task<FinishGameResult> FinishGameAsync(string clientId);
        Task<HandleTimeoutResult> HandlePendingTimeoutAsync(PendingPlayers pendingPlayers);
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
            player.PlayerState = ClientState.WaitingInQueue;

            _matchmakingRepository.UpdatePlayer(clientId, player);
            var countInQueue = _matchmakingRepository.CountInQueue;

            if (countInQueue >= PLAYERS_TO_START_GAME)
            {
                var players = _matchmakingRepository.GetFirstTwoInQueue().ToArray();
                players[0].PlayerState = ClientState.PendingMatchWaitingForConfirmation;
                players[1].PlayerState = ClientState.PendingMatchWaitingForConfirmation;
                foreach (var playerInGame in players)
                    _matchmakingRepository.UpdatePlayer(playerInGame.PlayerId, playerInGame);

                var pendingPlayers = new PendingPlayers(players);

                var result = new JoinQueueResult { PendingPlayers = pendingPlayers};
                return result;
            }

            return new JoinQueueResult { Player = player };
        }

        public LeaveQueueResult LeaveQueue(string clientId)
        {
            var player = _matchmakingRepository.GetByPlayerId(clientId);
            player.PlayerState = ClientState.Connected;
            _matchmakingRepository.UpdatePlayer(clientId, player);
            return new LeaveQueueResult { Player = player };
        }

        public Player CreatePlayer()
        {
            var player = new Player() { PlayerId = "PlayerId" + Guid.NewGuid().ToString(), PlayerState = ClientState.Connected };
            _matchmakingRepository.AddPlayer(player);
            return player;
        }

        public async Task<ConfirmGameResult> ConfirmGameAsync(string clientId)
        {
            var result = new ConfirmGameResult();
            //set player to confirmed
            var player = _matchmakingRepository.GetByPlayerId(clientId);
            player.PlayerState = ClientState.PendingMatchPlayerConfirmed;

            if (player == null) throw new Exception($"Player {clientId} not found");

            var pendingPlayers = _matchmakingRepository.FindPendingPlayers(clientId);

            if (pendingPlayers == null) throw new Exception($"PendingPlayer entity searched by playerId: {clientId} not found");

            //two players confirmed
            if (pendingPlayers.Players.All(x=>x.PlayerState == ClientState.PendingMatchPlayerConfirmed))
            {
                _matchmakingRepository.RemovePendingPlayers(pendingPlayers);
                var players = pendingPlayers.Players;
                foreach (var pl in players) 
                    _matchmakingRepository.RemovePlayer(pl.PlayerId);
                var game = _gameFactory.CreateGame(players);
                players[0].PlayerState = ClientState.InGamePlayerFirstMove;
                players[1].PlayerState = ClientState.InGameOpponentMove;
                result.Game = game;
                await _gameRepository.AddAsync(_converter.ToGameDbModel(game));
                return result;
            }

            //one player confirmed
            result.Player = player;
            return result;
        }

        public async Task<HandleTimeoutResult> HandlePendingTimeoutAsync(PendingPlayers pendingPlayers)
        {
            var result = new HandleTimeoutResult();

            //player who confirmed - back to join queue
            //player who not confirmed - back to connected
            foreach (var player in pendingPlayers.Players)
            {
                if(player.PlayerState == ClientState.PendingMatchPlayerConfirmed)
                {
                    player.PlayerState = ClientState.WaitingInQueue;
                }
                else if(player.PlayerState == ClientState.PendingMatchWaitingForConfirmation)
                {
                    player.PlayerState = ClientState.Connected;
                }
            }

            result.PendingPlayers = pendingPlayers;
            return result;
        }

        public async Task<FinishGameResult> FinishGameAsync(string clientId)
        {
            var result = new FinishGameResult();
            var game = await _gameRepository.GetByPlayerIdAsync(clientId);
            await _gameRepository.RemoveAsync(game.Id);
            var player = _matchmakingRepository.GetByPlayerId(clientId);
            player.PlayerState = ClientState.Connected;
            result.Player = player;
            return result;
        }
    }
}