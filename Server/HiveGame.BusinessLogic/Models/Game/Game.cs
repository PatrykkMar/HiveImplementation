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
        public Game(HiveBoard board, long gameId, Player[] players, PlayerColor startingColor = PlayerColor.White)
        {
            Graph = board;
            Players = players;
            GameId = gameId;
            StartingColor = startingColor;
        }

        public Game()
        {

        }

        public HiveBoard Graph { get; set; }
        public long GameId { get; set; }
        public Player[] Players { get; set; }
        public PlayerColor StartingColor { get; set; }
    }
}
