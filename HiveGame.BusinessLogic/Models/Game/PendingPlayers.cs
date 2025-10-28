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

        public bool IsPlayerThere(string id)
        {
            return Players.Any(x => x.PlayerId == id);
        }

        public PendingPlayers(Player[] players) 
        {
            this.Players = players;
            CreatingTime = DateTime.Now;
            Guid = Guid.NewGuid();
        }

        public int NumberOfConfirmedPlayers()
        {
            return Players.Count(x => x.PlayerState == ClientState.PendingMatchPlayerConfirmed);
        }

        public Player? GetOtherPlayer(string playerId)
        {
            return Players.FirstOrDefault(x=>x.PlayerId != playerId);
        }
    }
}
