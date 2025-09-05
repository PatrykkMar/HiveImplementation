using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveGame.DataAccess.Models
{
    public class PlayerDbModel
    {
        [BsonElement("playerId")]
        public string PlayerId { get; set; }
        [BsonElement("playerState")]
        public string PlayerState { get; set; }

        [BsonElement("playerColor")]
        public string PlayerColor { get; set; }
        [BsonElement("playerNick")]
        public string PlayerNick { get; set; }

        [BsonElement("playerInsects")]
        public Dictionary<string, int> PlayerInsects { get; set; }
    }
}
