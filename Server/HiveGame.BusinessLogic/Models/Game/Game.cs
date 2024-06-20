using HiveGame.BusinessLogic.Models.Game.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace HiveGame.BusinessLogic.Models.Game
{
    public class Game
    {
        public Game(HiveBoard board, long gameId, string[] players)
        {
            Graph = board;
            Players = players;
            GameId = gameId;
        }

        public Game()
        {

        }

        public HiveBoard Graph { get; set; }
        public long GameId { get; set; }
        public string[] Players { get; set; }
    }
}
