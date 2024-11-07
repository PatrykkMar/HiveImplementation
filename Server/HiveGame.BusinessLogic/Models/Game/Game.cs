using AutoMapper;
using HiveGame.BusinessLogic.Factories;
using HiveGame.BusinessLogic.Models.Board;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace HiveGame.BusinessLogic.Models
{
    public class Game
    {
        public Game(Player[] players, PlayerColor startingColor = PlayerColor.White)
        {
            Id = Guid.NewGuid().ToString();
            Board = new HiveBoard();
            Players = players;
            CurrentColorMove = startingColor;
            players[0].PlayerColor = PlayerColor.White;
            players[1].PlayerColor = PlayerColor.Black;
            NumberOfMove = 0;
        }

        public int NumberOfMove { get; set; }
        public int Turn
        {
            get
            {
                return NumberOfMove / 2 + 1;
            }
        }
        public string Id { get; set; }
        public HiveBoard Board { get; set; }
        public Player[] Players { get; set; }
        public PlayerColor CurrentColorMove { get; set; }
        public Player GetCurrentPlayer()
        {
            return Players.First(x => x.PlayerColor == CurrentColorMove);
        }
        public Player GetOtherPlayer()
        {
            return Players.First(x => x.PlayerColor != CurrentColorMove);
        }

        public PlayerViewDTO GetPlayerView(string playerId)
        {
            var playerViewDTO = new PlayerViewDTO();
            var player = Players.FirstOrDefault(x=>x.PlayerId == playerId);
            playerViewDTO.PlayerInsects = player.PlayerInsects;
            playerViewDTO.Board = BoardDTOFactory.CreateBoardDTO(Board, CurrentColorMove, Turn);
            return playerViewDTO;
        }

        public void AfterActionMade()
        {
            CurrentColorMove = (PlayerColor)(((int)(CurrentColorMove + 1)) % 2);
            NumberOfMove++;
        }

        public bool CheckGameOverCondition()
        {
            //if queen is surrounded, game is over
            var queensVertices = Board.Vertices.Where(x => x.InsectStack.Any(x => x.Type == Insects.InsectType.Queen));

            var surroundedQueens = queensVertices.Where(x => Board.GetAdjacentVerticesByCoordList(x).Where(x=>!x.IsEmpty).Count() == 6);

            var numberOfSurroundedQueens = surroundedQueens.Count();

            if (numberOfSurroundedQueens > 0)
                return true;

            return false;
        }

    }
}
