using HiveGame.BusinessLogic.Models;
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
        Game? GetByPlayerId(string playerId);

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
            throw new NotImplementedException();
        }

        public bool Update(string gameId, Game updatedItem)
        {
            throw new NotImplementedException();
        }

        public bool Remove(string gameId)
        {
            throw new NotImplementedException();
        }

        public Game? GetByPlayerId(string playerId)
        {
            return _items.FirstOrDefault(x => x.Players.Select(x => x.PlayerId).Contains(playerId));
        }
    }
}
