using HiveGame.BusinessLogic.Factories;
using HiveGame.BusinessLogic.Models.Game.Graph;
using HiveGame.BusinessLogic.Models.Graph;
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
        public Game(HiveBoard board, string gameId, Player[] players, PlayerColor startingColor = PlayerColor.White)
        {
            Graph = board;
            Players = players;
            GameId = gameId;
            StartingColor = startingColor;
        }

        public Game(string gameId, Player[] players, IInsectFactory factory, PlayerColor startingColor = PlayerColor.White)
        {
            Graph = new HiveBoard(factory);
            Players = players;
            GameId = gameId;
            StartingColor = startingColor;
        }

        public HiveBoard Graph { get; set; }
        public string GameId { get; set; }
        public Player[] Players { get; set; }
        public PlayerColor StartingColor { get; set; }
    }
}
