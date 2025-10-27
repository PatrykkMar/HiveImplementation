using HiveGame.BusinessLogic.Models;
using HiveGame.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveGame.BusinessLogic.Repositories
{
    public interface IMatchmakingRepository
    {
        long CountInQueue { get; }

        void AddPlayer(Player item);

        IEnumerable<Player> GetAll();

        Player? GetByPlayerId(string playerId);

        bool UpdatePlayer(string playerId, Player updatedItem);

        List<Player> GetFirstTwoInQueue();

        bool RemovePlayer(string playerId);

        void AddPendingPlayers(PendingPlayers item);

        void RemovePendingPlayers(PendingPlayers item);

        PendingPlayers? FindPendingPlayers(string playerId);
        List<PendingPlayers> PendingPlayers { get; }
    }

    public class MatchmakingRepository : IMatchmakingRepository
    {

        private readonly List<Player> _players = new();
        private readonly List<PendingPlayers> _pendingPlayers = new();
        private readonly object _pendingPlayersLock = new(); // 🔒 obiekt do blokowania

        public List<PendingPlayers> PendingPlayers
        {
            get
            {
                lock (_pendingPlayersLock)
                {
                    return new List<PendingPlayers>(_pendingPlayers);
                }
            }
        }

        public MatchmakingRepository()
        {
        }

        public long CountInQueue
        {
            get { return _players.Where(x=>x.PlayerState == ClientState.WaitingInQueue).Count(); }
        }

        public void AddPlayer(Player item)
        {
            _players.Add(item);
        }

        public IEnumerable<Player> GetAll()
        {
            return _players;
        }

        public Player? GetByPlayerId(string playerId)
        {
            return _players.FirstOrDefault(x => x.PlayerId == playerId);
        }

        public bool UpdatePlayer(string playerId, Player updatedItem)
        {
            var index = _players.FindIndex(x => x.PlayerId == playerId);
            if (index == -1)
            {
                return false;
            }

            _players[index] = updatedItem;
            return true;
        }


        public List<Player> GetFirstTwoInQueue()
        {
            var firstTwoItems = _players.Where(x=>x.PlayerState == ClientState.WaitingInQueue).Take(2).ToList();
            return firstTwoItems;
        }

        public bool RemovePlayer(string playerId)
        {
            var item = _players.FirstOrDefault(x => x.PlayerId == playerId);
            if (item == null)
            {
                return false;
            }

            _players.Remove(item);
            return true;
        }

        public void AddPendingPlayers(PendingPlayers item)
        {
            lock (_pendingPlayersLock)
            {
                _pendingPlayers.Add(item);
            }
        }

        public void RemovePendingPlayers(PendingPlayers item)
        {
            lock (_pendingPlayersLock)
            {
                _pendingPlayers.Remove(item);
            }
        }

        public PendingPlayers? FindPendingPlayers(string playerId)
        {
            lock (_pendingPlayersLock)
            {
                return _pendingPlayers.FirstOrDefault(x => x.IsPlayerThere(playerId));
            }
        }
    }
}