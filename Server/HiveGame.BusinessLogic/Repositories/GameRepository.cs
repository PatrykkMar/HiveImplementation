using HiveGame.BusinessLogic.Models.Game;
using System;
using System.Collections.Generic;

namespace HiveGame.BusinessLogic.Repositories
{
    public interface IGameRepository
    {
        long Count { get; }

        void Add(Game item);

        IEnumerable<Game> GetAll();

        Game? GetByGameId(string gameId);

        bool Update(string gameId, Game updatedItem);

        bool Remove(string gameId);
    }

    public class GameRepository : IGameRepository
    {
        private readonly List<Game> _items;

        public GameRepository()
        {
            _items = new List<Game>();
        }

        public long Count
        {
            get { return _items.Count; }
        }

        public void Add(Game item)
        {
            _items.Add(item);
        }

        public IEnumerable<Game> GetAll()
        {
            return _items;
        }

        public Game? GetByGameId(string gameId)
        {
            return _items.FirstOrDefault(x => x.GameId == gameId);
        }

        public bool Update(string gameId, Game updatedItem)
        {
            var index = _items.FindIndex(x => x.GameId == gameId);
            if (index == -1)
            {
                return false;
            }

            _items[index] = updatedItem;
            return true;
        }

        public bool Remove(string gameId)
        {
            var item = _items.FirstOrDefault(x => x.GameId == gameId);
            if (item == null)
            {
                return false;
            }

            _items.Remove(item);
            return true;
        }
    }
}
