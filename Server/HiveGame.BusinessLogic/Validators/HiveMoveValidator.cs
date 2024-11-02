using AutoMapper;
using HiveGame.BusinessLogic.Models;
using HiveGame.BusinessLogic.Models;
using HiveGame.BusinessLogic.Models.Insects;
using HiveGame.BusinessLogic.Models.Requests;
using System.Net.WebSockets;
using HiveGame.BusinessLogic.Models.WebSocketModels;
using HiveGame.BusinessLogic.Factories;
using HiveGame.BusinessLogic.Models.Board;
using HiveGame.BusinessLogic.Repositories;

namespace HiveGame.BusinessLogic.Services
{

    public interface IHiveMoveValidator
    {
        public void ValidateMove(MoveInsectRequest request);
        public void ValidatePut(PutInsectRequest request);
        public void ValidatePutFirstInsect(PutFirstInsectRequest request);
    }

    public class HiveMoveValidator : IHiveMoveValidator
    {
        private readonly IGameRepository _gameRepository;

        public HiveMoveValidator(IGameRepository gameRepository)
        {
            _gameRepository = gameRepository;
        }

        public void ValidateMove(MoveInsectRequest request)
        {
            Game? game = _gameRepository.GetByPlayerId(request.PlayerId);

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

            if (!request.MoveFrom.HasValue)
                throw new ArgumentException("Empty moveFrom parameter");

            if (!request.MoveTo.HasValue)
                throw new ArgumentException("Empty moveTo parameter");

            var moveFromVertex = board.GetVertexByCoord(request.MoveFrom.Value);
            var moveToVertex = board.GetVertexByCoord(request.MoveTo.Value);

            if (moveFromVertex == null || moveFromVertex.IsEmpty)
                throw new ArgumentException("MoveFromVertex not found or empty");

            if (moveToVertex == null)
                throw new ArgumentException("MoveToVertex not found or not empty");

            if (moveFromVertex.CurrentInsect.PlayerColor != game.GetCurrentPlayer().PlayerColor)
                throw new ArgumentException("Player wants to move opponent's insect");

            var availableHexes = moveFromVertex.CurrentInsect.GetAvailableVertices(moveFromVertex, board);

            if (!availableHexes.Contains(moveToVertex))
                throw new ArgumentException("Insect cannot move there");
        }

        public void ValidatePut(PutInsectRequest request)
        {
            Game? game = _gameRepository.GetByPlayerId(request.PlayerId);

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

            if (game.NumberOfMove == 4 && board.AllInsects.FirstOrDefault(x => x.Type == InsectType.Queen && x.PlayerColor == game.GetCurrentPlayer().PlayerColor) == null)
                throw new Exception("It's 4 turn and you still didn't put a queen");
        }

        public void ValidatePutFirstInsect(PutFirstInsectRequest request)
        {
            Game? game = _gameRepository.GetByPlayerId(request.PlayerId);

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