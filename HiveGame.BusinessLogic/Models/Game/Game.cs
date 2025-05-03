using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using HiveGame.BusinessLogic.Models.Board;
using System.Linq;
using HiveGame.Core.Models;
using HiveGame.Core.Extensions;

namespace HiveGame.BusinessLogic.Models
{
    public interface IGame
    {
        string Id { get; set; }
        int NumberOfMove { get; set; }
        int Turn { get; }
        IHiveBoard Board { get; set; }
        Player[] Players { get; set; }
        PlayerColor CurrentColorMove { get; set; }

        Player GetCurrentPlayer();
        Player GetOtherPlayer();
        PlayerViewDTO GetPlayerView(string playerId);
        void AfterActionMade();
        bool CheckGameOverCondition();
    }

    public class Game : IGame
    {
        public Game(Player[] players, PlayerColor startingColor = PlayerColor.White)
        {
            Id = ObjectId.GenerateNewId().ToString();
            Board = new HiveBoard();
            Players = players;
            CurrentColorMove = startingColor;
            //TODO: Randoming first player
            //players = players.Shuffle().ToArray();
            players[0].PlayerColor = PlayerColor.White;
            players[1].PlayerColor = PlayerColor.Black;
            NumberOfMove = 0;
        }


        public string Id { get; set; }

        public int NumberOfMove { get; set; }

        public int Turn => NumberOfMove / 2 + 1;

        public IHiveBoard Board { get; set; }

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
            var player = Players.FirstOrDefault(x => x.PlayerId == playerId);
            playerViewDTO.PlayerInsectTypePairs = player.PlayerInsects.Select(x => new PlayerInsectTypePairDTO { type = x.Key, amount = x.Value }).ToList();
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
            // Check if any queen is surrounded, indicating game over.
            var queensVertices = Board.Vertices.Where(x => x.InsectStack.Any(x => x.Type == InsectType.Queen));
            var surroundedQueens = queensVertices.Where(x => Board.GetAdjacentVerticesByCoordList(x).Count(v => !v.IsEmpty) == 6);
            return surroundedQueens.Any();
        }
    }
}