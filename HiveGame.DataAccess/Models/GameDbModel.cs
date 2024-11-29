using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveGame.DataAccess.Models
{
    public class GameDbModel
    {
        [BsonId]
        public string Id { get; set; }

        [BsonElement("numberOfMove")]
        public int NumberOfMove { get; set; }

        [BsonElement("currentColorMove")]
        public string CurrentColorMove { get; set; }

        [BsonElement("board")]
        public List<VertexDbModel> Board { get; set; }

        [BsonElement("players")]
        public List<PlayerDbModel> Players { get; set; }
    }

}
