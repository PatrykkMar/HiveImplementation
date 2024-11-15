using HiveGame.BusinessLogic.Models;
using HiveGame.BusinessLogic.Models.Insects;
using HiveGame.BusinessLogic.Models.Requests;
using HiveGame.BusinessLogic.Repositories;

namespace HiveGame.BusinessLogic.Services
{

    public interface IHiveMoveValidator
    {
        public void ValidateMove(MoveInsectRequest request, Game? game);
        public void ValidatePut(PutInsectRequest request, Game? game);
        public void ValidatePutFirstInsect(PutFirstInsectRequest request, Game? game);
    }

    public class HiveMoveValidator : IHiveMoveValidator
    {

        public HiveMoveValidator()
        {
        }

        public void ValidateMove(MoveInsectRequest request, Game? game)
        {
            if (game == null)
                throw new Exception("Game not found");

            var currentPlayer = game.GetCurrentPlayer();

            if (currentPlayer == null)
                throw new Exception("Player not found in the game");

            if (request.PlayerId != currentPlayer?.PlayerId)
            {
                throw new Exception("It's not your move");
            }

            var board = game.Board;

            if (request.MoveFrom == null)
                throw new ArgumentException("Empty moveFrom parameter");

            if (request.MoveTo == null)
                throw new ArgumentException("Empty moveTo parameter");

            var moveFromVertex = board.GetVertexByCoord(request.MoveFrom);
            var moveToVertex = board.GetVertexByCoord(request.MoveTo);

            if (moveFromVertex == null || moveFromVertex.IsEmpty)
                throw new ArgumentException("MoveFromVertex not found or empty");

            if (moveToVertex == null)
                throw new ArgumentException("MoveToVertex not found or not empty");

            if (moveFromVertex.CurrentInsect.PlayerColor != game.GetCurrentPlayer().PlayerColor)
                throw new ArgumentException("Player wants to move opponent's insect");

            var availableHexes = moveFromVertex.CurrentInsect.GetAvailableVertices(moveFromVertex, board);

            if (!availableHexes.AvailableVertices.Contains(moveToVertex))
                throw new ArgumentException("Insect cannot move there");
        }

        public void ValidatePut(PutInsectRequest request, Game? game)
        {
            if (game == null)
                throw new Exception("Game not found");

            var currentPlayer = game.GetCurrentPlayer();

            if (currentPlayer == null)
                throw new Exception("Player not found in the game");

            if (request.PlayerId != currentPlayer?.PlayerId)
            {
                throw new Exception("It's not your move");
            }

            var board = game.Board;

            if (board.FirstMoves)
            {
                throw new Exception("You have to put first insects first");
            }

            if (request.InsectToPut == InsectType.Nothing)
                throw new ArgumentNullException("Insect not specified");

            if (!game.GetCurrentPlayer().RemoveInsectFromPlayerBoard(request.InsectToPut))
                throw new Exception("Player can't put this insect");

            if (game.Turn == 4 && board.AllInsects.FirstOrDefault(x => x.Type == InsectType.Queen && x.PlayerColor == game.GetCurrentPlayer().PlayerColor) == null && request.InsectToPut != InsectType.Queen)
                throw new Exception("It's 4 turn and you still didn't put a queen");
        }

        public void ValidatePutFirstInsect(PutFirstInsectRequest request, Game? game)
        {
            if (game == null)
                throw new Exception("Game not found");

            var currentPlayer = game.GetCurrentPlayer();

            if (currentPlayer == null)
                throw new Exception("Player not found in the game");

            if (request.PlayerId != currentPlayer?.PlayerId)
            {
                throw new Exception("It's not your move");
            }

            var board = game.Board;

            if (!board.FirstMoves)
            {
                throw new Exception("There are first insects put");
            }

            if (!currentPlayer.RemoveInsectFromPlayerBoard(request.InsectToPut))
                throw new Exception("Player can't put this insect");
        }
    }
}