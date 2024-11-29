using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveGame.DataAccess.Models
{
    public class VertexDbModel
    {
        [BsonElement("id")]
        public long Id { get; set; }

        [BsonElement("x")]
        public int X { get; set; }

        [BsonElement("y")]
        public int Y { get; set; }

        [BsonElement("insectStack")]
        public List<InsectDbModel> InsectStack { get; set; }
    }
}
