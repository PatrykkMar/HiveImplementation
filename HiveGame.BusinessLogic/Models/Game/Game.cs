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
        bool OnOneComputer { get; set; }
        IHiveBoard Board { get; set; }
        Player[] Players { get; set; }
        Player[] GetPlayersToSendState { get;}
        PlayerColor CurrentColorMove { get; }

        Player? GetPlayerById(string playerId);
        Player GetCurrentPlayer();
        Player GetWaitingPlayer();
        Player GetOtherPlayer(string playerId);
        PlayerViewDTO GetPlayerView(string playerId);
        void AfterActionMade();
        bool GameOverConditionMet();
    }

    public class Game : IGame
    {
        public Game(Player[] players, PlayerColor startingColor = PlayerColor.White, string? gameId = null, bool onOneComputer = false)
        {
            Id = string.IsNullOrEmpty(gameId) ? ObjectId.GenerateNewId().ToString() : gameId;
            Board = new HiveBoard();
            Players = players;
            CurrentColorMove = startingColor;
            //TODO: Randoming first player
            //players = players.Shuffle().ToArray();
            players[0].PlayerColor = PlayerColor.White;
            players[1].PlayerColor = PlayerColor.Black;
            NumberOfMove = 0;
            OnOneComputer = onOneComputer;
        }


        public string Id { get; set; }

        public int NumberOfMove { get; set; }

        public int Turn => NumberOfMove / 2 + 1;

        public IHiveBoard Board { get; set; }

        public Player[] Players { get; set; }

        public Player[] GetPlayersToSendState { get
            {
                return !OnOneComputer ? Players : Players.Where(x=>x.PlayerState!=ClientState.InGameOpponentMove).ToArray();
            } }

        public PlayerColor CurrentColorMove { get; private set; }
        public bool OnOneComputer { get; set; }

        public Player GetCurrentPlayer()
        {
            return Players.First(x => x.PlayerColor == CurrentColorMove);
        }

        public Player GetWaitingPlayer()
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
            //setting next color
            switch (CurrentColorMove)
            {
                case PlayerColor.Black:
                    CurrentColorMove = PlayerColor.White;
                    break;
                case PlayerColor.White:
                    CurrentColorMove = PlayerColor.Black;
                    break;
                case PlayerColor.NoColor:
                    throw new Exception("There cannot be current player without a color");
            }

            NumberOfMove++;
        }

        public bool GameOverConditionMet()
        {
            // Check if any queen is surrounded, indicating game over.
            var queensVertices = Board.Vertices.Where(x => x.InsectStack.Any(x => x.Type == InsectType.Queen));
            var surroundedQueens = queensVertices.Where(x => Board.GetAdjacentVerticesByCoordList(x).Count(v => !v.IsEmpty) == 6);
            return surroundedQueens.Any();
        }

        public Player GetOtherPlayer(string playerId)
        {
            return Players.First(x => x.PlayerId != playerId);
        }

        public Player? GetPlayerById(string playerId)
        {
            return Players.FirstOrDefault(x => x.PlayerId == playerId);
        }
    }
}