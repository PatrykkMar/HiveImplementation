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
        public string? GameId { get; set; }
        public string? PlayerId { get; set; }
        public PlayerColor PlayerColor { get; set; }
    }

    public enum PlayerColor
    {
        White, Black
    }
}
