using HiveGame.BusinessLogic.Models;
using HiveGame.BusinessLogic.Repositories;
using HiveGame.Handlers;

namespace HiveGame.Utils
{
    /// <summary>
    /// Monitors pending players waiting for match confirmation.
    /// If players don't confirm within a given timeout, triggers timeout logic via GameActionsResponseHandler.
    /// </summary>
    public class PendingPlayersWatcher : BackgroundService
    {
        private readonly IMatchmakingRepository _matchmakingRepository;
        private readonly ILogger<PendingPlayersWatcher> _logger;
        private const int TIMEOUT_SECONDS = 10;
        private const int LOOP_INTERVAL_MS = 500;

        public event Func<PendingPlayers, Task>? OnTimeout;

        public PendingPlayersWatcher(
            ILogger<PendingPlayersWatcher> logger,
            IMatchmakingRepository repository)
        {
            _matchmakingRepository = repository;
            _logger = logger;
        }

        /// <summary>
        /// Background loop — checks pending players for timeouts.
        /// </summary>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("PendingPlayersWatcher started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.Now;

                foreach (var entity in _matchmakingRepository.PendingPlayers)
                {
                    if ((now - entity.CreatingTime).TotalSeconds >= TIMEOUT_SECONDS)
                    {
                        RemovePendingPlayers(entity);
                        OnTimeout?.Invoke(entity);
                    }
                }

                await Task.Delay(LOOP_INTERVAL_MS, stoppingToken);
            }

            _logger.LogInformation("PendingPlayersWatcher stopped.");
        }

        public void RemovePendingPlayers(PendingPlayers players)
        {
            _matchmakingRepository.PendingPlayers.Remove(players);
        }

        public void AddPendingPlayers(PendingPlayers players)
        {
            players.CreatingTime = DateTime.Now;
            _matchmakingRepository.PendingPlayers.Add(players);
        }
    }
}