using HiveGame.BusinessLogic.Models;
using HiveGame.BusinessLogic.Context;
using MongoDB.Driver;
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
        private readonly IMongoCollection<Game> _games;

        public GameRepository(MongoDBContext context)
        {
            _games = context.Games;
        }

        public long Count
        {
            get { return _games.CountDocuments(_ => true); }
        }

        public void Add(Game item)
        {
            _games.InsertOne(item);
        }

        public IEnumerable<Game> GetAll()
        {
            return _games.Find(_ => true).ToList();
        }

        public Game? GetByGameId(string gameId)
        {
            return _games.Find(game => game.Id == gameId).FirstOrDefault();
        }

        public Game? GetByPlayerId(string playerId)
        {
            return _games.Find(game => game.Players.Any(player => player.PlayerId == playerId)).FirstOrDefault();
        }

        public bool Update(string gameId, Game updatedItem)
        {
            var result = _games.ReplaceOne(game => game.Id == gameId, updatedItem);
            return result.ModifiedCount > 0;
        }

        public bool Remove(string gameId)
        {
            var result = _games.DeleteOne(game => game.Id == gameId);
            return result.DeletedCount > 0;
        }
    }
}