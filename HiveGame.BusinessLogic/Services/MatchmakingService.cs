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

        Task ConfirmGame(string clientId);
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

                var pendingPlayers = players;

                var result = new JoinQueueResult { PendingPlayers = pendingPlayers };
                await WaitingForPendingPlayerAsync(pendingPlayers);
                return result;
            }

            return new JoinQueueResult { Player = player };
        }

        public async Task WaitingForPendingPlayerAsync(Player[] pendingPlayers)
        {
            Player[] players = pendingPlayers;
            _ = Task.Run(async () =>
            {
                var timeout = TimeSpan.FromSeconds(10);
                var start = DateTime.UtcNow;

                while (DateTime.UtcNow - start < timeout)
                {
                    if (pendingPlayers.All(x => x.PlayerState == ClientState.PendingMatchPlayerConfirmed))
                    {
                        var game = _converter.ToGameDbModel(_gameFactory.CreateGame(players));
                        //players to game

                        return;
                    }
                    await Task.Delay(500);
                }

                //await HandlePendingTimeoutAsync(pendingPlayers);
            });

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

        public async Task ConfirmGame(string clientId)
        {
            //set player to confirmed
        }

        public async Task HandlePendingTimeoutAsync(Player[] pendingPlayers)
        {
            //player who confirmed - back to join queue
            //player who not confirmed - back to connected
        }
    }
}