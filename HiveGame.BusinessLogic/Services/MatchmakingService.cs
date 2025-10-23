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
        Task ConfirmGameAsync(string clientId);
        Task WaitingForPendingPlayerAsync(PendingPlayers pendingPlayers);
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
                var players = _matchmakingRepository.GetAndRemoveFirstTwoInQueue().ToArray();
                players[0].PlayerState = ClientState.PendingMatchWaitingForConfirmation;
                players[1].PlayerState = ClientState.PendingMatchWaitingForConfirmation;
                foreach (var playerInGame in players)
                    _matchmakingRepository.UpdatePlayer(playerInGame.PlayerId, playerInGame);

                var pendingPlayers = new PendingPlayers(players);

                var result = new JoinQueueResult { PendingPlayers = pendingPlayers};
                _matchmakingRepository.AddPendingPlayers(pendingPlayers);
                await WaitingForPendingPlayerAsync(pendingPlayers);
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
            var player = new Player() { PlayerId = Guid.NewGuid().ToString(), PlayerState = ClientState.Connected };
            _matchmakingRepository.AddPlayer(player);
            return player;
        }

        public async Task ConfirmGameAsync(string clientId)
        {
            //set player to confirmed
            var player = _matchmakingRepository.GetByPlayerId(clientId);
            var pendingGame = _matchmakingRepository.FindPendingPlayers(player.PlayerId);
            player.PlayerState = ClientState.PendingMatchPlayerConfirmed;

            if (pendingGame.NumberOfConfirmedPlayers() == 2)
            {
                var players = pendingGame.Players;
                var game = _gameFactory.CreateGame(pendingGame.Players);
                players[0].PlayerState = ClientState.InGamePlayerFirstMove;
                players[1].PlayerState = ClientState.InGameOpponentMove;
                foreach (var playerInGame in players)
                    _matchmakingRepository.UpdatePlayer(playerInGame.PlayerId, playerInGame);

                var result = new ConfirmGameResult { Game = game };
                await _gameRepository.AddAsync(_converter.ToGameDbModel(game));
            }
            else
            {
                //inform about confirming
            }

            if (player == null) throw new Exception($"Player {clientId} not found");
        }

        public async Task WaitingForPendingPlayerAsync(PendingPlayers pendingPlayers)
        {
            Player[] players = pendingPlayers.Players;
            _ = Task.Run(async () =>
            {
                var timeout = TimeSpan.FromSeconds(10);
                var start = DateTime.UtcNow;

                while (DateTime.UtcNow - start < timeout)
                {
                    if (players.All(x => x.PlayerState == ClientState.PendingMatchPlayerConfirmed))
                    {
                        var game = _converter.ToGameDbModel(_gameFactory.CreateGame(players));
                        await _gameRepository.AddAsync(game);
                        //players to game

                        return;
                    }
                    await Task.Delay(500);
                }

                await HandlePendingTimeoutAsync(pendingPlayers);
            });

        }

        public async Task HandlePendingTimeoutAsync(PendingPlayers pendingPlayers)
        {
            //player who confirmed - back to join queue
            //player who not confirmed - back to connected
            foreach(var player in pendingPlayers.Players)
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
        }
    }
}