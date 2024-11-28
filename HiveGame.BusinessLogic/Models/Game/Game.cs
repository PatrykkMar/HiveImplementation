using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using HiveGame.BusinessLogic.Models.Board;
using System.Linq;
using HiveGame.Core.Models;

namespace HiveGame.BusinessLogic.Models
{
    public class Game
    {
        public Game(Player[] players, PlayerColor startingColor = PlayerColor.White)
        {
            Id = ObjectId.GenerateNewId().ToString();
            Board = new HiveBoard();
            Players = players;
            CurrentColorMove = startingColor;
            players[0].PlayerColor = PlayerColor.White;
            players[1].PlayerColor = PlayerColor.Black;
            NumberOfMove = 0;
        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public int NumberOfMove { get; set; }

        public int Turn => NumberOfMove / 2 + 1;

        public HiveBoard Board { get; set; }

        public Player[] Players { get; set; }
        [BsonRepresentation(BsonType.String)]
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
            // Check if any queen is surrounded, indicating game over.
            var queensVertices = Board.Vertices.Where(x => x.InsectStack.Any(x => x.Type == InsectType.Queen));
            var surroundedQueens = queensVertices.Where(x => Board.GetAdjacentVerticesByCoordList(x).Count(v => !v.IsEmpty) == 6);
            return surroundedQueens.Any();
        }
    }
}