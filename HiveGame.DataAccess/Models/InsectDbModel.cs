using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveGame.DataAccess.Models
{
    public class InsectDbModel
    {
        [BsonElement("type")]
        public string Type { get; set; }

        [BsonElement("playerColor")]
        public string PlayerColor { get; set; }
    }
}
