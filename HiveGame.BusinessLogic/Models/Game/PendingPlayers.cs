using HiveGame.BusinessLogic.Models.Insects;
using HiveGame.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace HiveGame.BusinessLogic.Models
{
    public class PendingPlayers
    {
        public Player[] Players { get; set; }
        public DateTime CreatingTime { get; set; }
        public Guid Guid { get; set; }

        public bool IsPlayerThere(string nick)
        {
            return Players.Any(x => x.PlayerNick == nick);
        }

        public PendingPlayers(Player[] players) 
        {
            this.Players = Players;
            CreatingTime = DateTime.Now;
            Guid = Guid.NewGuid();
        }

        public int NumberOfConfirmedPlayers()
        {
            return Players.Count(x => x.PlayerState == ClientState.PendingMatchPlayerConfirmed);
        }
    }
}
