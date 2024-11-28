using HiveGame.BusinessLogic.Models.Board;
using HiveGame.BusinessLogic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson;
using HiveGame.BusinessLogic.Models.Insects;
using HiveGame.Core.Models;

namespace HiveGame.BusinessLogic.Context
{
    public class MongoDBContext
    {
        private readonly IMongoDatabase _database;

        public MongoDBContext(IOptions<DatabaseSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.DatabaseName);
            BsonSerializer.RegisterSerializer(new EnumSerializer<InsectType>(BsonType.String));
        }

        public IMongoCollection<Game> Games => _database.GetCollection<Game>("games");

    }


    public class DatabaseSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}
