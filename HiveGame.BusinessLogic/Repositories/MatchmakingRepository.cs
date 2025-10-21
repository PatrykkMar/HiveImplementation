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

        List<Player> GetAndRemoveFirstTwoInQueue();

        bool RemovePlayer(string playerId);
    }

    public class MatchmakingRepository : IMatchmakingRepository
    {

        private readonly List<Player> _items;

        public MatchmakingRepository()
        {
            _items = new List<Player>();
        }

        public long CountInQueue
        {
            get { return _items.Where(x=>x.PlayerState == ClientState.WaitingInQueue).Count(); }
        }

        public void AddPlayer(Player item)
        {
            _items.Add(item);
        }

        public IEnumerable<Player> GetAll()
        {
            return _items;
        }

        public Player? GetByPlayerId(string playerId)
        {
            return _items.FirstOrDefault(x => x.PlayerId == playerId);
        }

        public bool UpdatePlayer(string playerId, Player updatedItem)
        {
            var index = _items.FindIndex(x => x.PlayerId == playerId);
            if (index == -1)
            {
                return false;
            }

            _items[index] = updatedItem;
            return true;
        }


        public List<Player> GetAndRemoveFirstTwoInQueue()
        {
            var firstTwoItems = _items.Where(x=>x.PlayerState == ClientState.WaitingInQueue).Take(2).ToList();
            foreach (var item in firstTwoItems)
            {
                _items.Remove(item);
            }
            return firstTwoItems;
        }

        public bool RemovePlayer(string playerId)
        {
            var item = _items.FirstOrDefault(x => x.PlayerId == playerId);
            if (item == null)
            {
                return false;
            }

            _items.Remove(item);
            return true;
        }
    }
}