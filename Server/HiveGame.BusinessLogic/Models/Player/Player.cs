using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace HiveGame.BusinessLogic.Models
{
    public class Player
    {
        public string Nick { get; set; }
        public WebSocket WebSocket { get; set; }
        public long? GameId { get; set; }
        public long PlayerId { get; set; }
    }

    public enum PlayerColor
    {
        White, Black
    }
}
