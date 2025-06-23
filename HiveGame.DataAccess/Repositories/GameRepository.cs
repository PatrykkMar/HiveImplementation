using MongoDB.Driver;
using System.Collections.Generic;
using HiveGame.DataAccess.Models;
using HiveGame.DataAccess.Context;

namespace HiveGame.DataAccess.Repositories
{
    public interface IGameRepository
    {
        Task<long> CountAsync();

        Task AddAsync(GameDbModel item);

        Task<IEnumerable<GameDbModel>> GetAllAsync();

        Task<GameDbModel?> GetByGameIdAsync(string gameId);
        Task<GameDbModel?> GetByPlayerIdAsync(string playerId);

        Task<bool> UpdateAsync(string gameId, GameDbModel updatedItem);

        Task<bool> RemoveAsync(string gameId);
    }

    public class GameRepository : IGameRepository
    {
        private readonly IMongoCollection<GameDbModel> _games;

        public GameRepository(MongoDBContext context)
        {
            _games = context.Games;
        }

        public async Task<long> CountAsync()
        {
            return await _games.CountDocumentsAsync(_ => true);
        }

        public async Task AddAsync(GameDbModel item)
        {
            await _games.InsertOneAsync(item);
        }

        public async Task<IEnumerable<GameDbModel>> GetAllAsync()
        {
            var result = await _games.FindAsync(_ => true);
            return result.ToList();
        }

        public async Task<GameDbModel?> GetByGameIdAsync(string gameId)
        {
            var result = await _games.FindAsync(game => game.Id == gameId);
            return await result.FirstOrDefaultAsync();
        }

        public async Task<GameDbModel?> GetByPlayerIdAsync(string playerId)
        {
            var result = await _games.FindAsync(game => game.Players.Any(player => player.PlayerId == playerId));
            return await result.FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateAsync(string gameId, GameDbModel updatedItem)
        {
            var filter = Builders<GameDbModel>.Filter.Eq(game => game.Id, gameId);
            var result = await _games.ReplaceOneAsync(filter, updatedItem);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> RemoveAsync(string gameId)
        {
            var result = await _games.DeleteOneAsync(game => game.Id == gameId);
            return result.DeletedCount > 0;
        }
    }
}