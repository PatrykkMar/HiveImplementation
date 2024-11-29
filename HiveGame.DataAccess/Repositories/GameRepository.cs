using MongoDB.Driver;
using System.Collections.Generic;
using HiveGame.DataAccess.Models;
using HiveGame.DataAccess.Context;

namespace HiveGame.DataAccess.Repositories
{
    public interface IGameRepository
    {
        long Count { get; }

        void Add(GameDbModel item);

        IEnumerable<GameDbModel> GetAll();

        GameDbModel? GetByGameId(string gameId);
        GameDbModel? GetByPlayerId(string playerId);

        bool Update(string gameId, GameDbModel updatedItem);

        bool Remove(string gameId);
    }

    public class GameRepository : IGameRepository
    {
        private readonly IMongoCollection<GameDbModel> _games;

        public GameRepository(MongoDBContext context)
        {
            _games = context.Games;
        }

        public long Count
        {
            get { return _games.CountDocuments(_ => true); }
        }

        public void Add(GameDbModel item)
        {
            _games.InsertOne(item);
        }

        public IEnumerable<GameDbModel> GetAll()
        {
            return _games.Find(_ => true).ToList();
        }

        public GameDbModel? GetByGameId(string gameId)
        {
            return _games.Find(game => game.Id == gameId).FirstOrDefault();
        }

        public GameDbModel? GetByPlayerId(string playerId)
        {
            return _games.Find(game => game.Players.Any(player => player.PlayerId == playerId)).FirstOrDefault();
        }

        public bool Update(string gameId, GameDbModel updatedItem)
        {
            var filter = Builders<GameDbModel>.Filter.Eq(game => game.Id, gameId);
            var result = _games.ReplaceOne(filter, updatedItem);
            return result.ModifiedCount > 0;
        }

        public bool Remove(string gameId)
        {
            var result = _games.DeleteOne(game => game.Id == gameId);
            return result.DeletedCount > 0;
        }
    }
}