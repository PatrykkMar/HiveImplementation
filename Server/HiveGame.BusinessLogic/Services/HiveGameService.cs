using HiveGame.BusinessLogic.Factories;
using HiveGame.BusinessLogic.Models.Graph;
using HiveGame.BusinessLogic.Models.Insects;
using HiveGame.BusinessLogic.Models.Requests;
using HiveGame.BusinessLogic.Models;
using HiveGame.BusinessLogic.Repositories;
using HiveGame.BusinessLogic.Services;

namespace HiveGame.BusinessLogic.Services
{
    public interface IHiveGameService
    {
        public HiveActionResult Move(MoveInsectRequest request);
        public HiveActionResult Put(PutInsectRequest request);
        public HiveActionResult PutFirstInsect(PutFirstInsectRequest request);
    }

    public class HiveGameService : IHiveGameService
    {
        private readonly IGameRepository _gameRepository;
        private readonly IInsectFactory _insectFactory;
        private readonly IHiveMoveValidator _moveValidator;

        public HiveGameService(IInsectFactory insectFactory, IGameRepository gameRepository, IHiveMoveValidator hiveMoveValidator)
        {
            _gameRepository = gameRepository;
            _insectFactory = insectFactory;
            _moveValidator = hiveMoveValidator;
        }

        public HiveActionResult Move(MoveInsectRequest request)
        {
            _moveValidator.ValidateMove(request);

            Game? game = _gameRepository.GetByPlayerId(request.PlayerId);
            var currentPlayer = game.GetCurrentPlayer();
            var board = game.Board;

            var moveFromVertex = board.GetVertexByCoord(request.MoveFrom.Value);
            var moveToVertex = board.GetVertexByCoord(request.MoveTo.Value);
            var moveToVertexEmptyBeforeMove = moveToVertex.IsEmpty;

            var moveFromInsect = moveFromVertex.InsectStack.Pop();
            moveToVertex.InsectStack.Push(moveFromInsect);

            if (moveFromVertex.IsEmpty)
            {
                board.RemoveAllEmptyUnconnectedVerticesAround(moveFromVertex);
            }

            if (moveToVertexEmptyBeforeMove)
            {
                board.AddEmptyVerticesAround(moveToVertex);
            }

            game.AfterActionMade();

            var result = new HiveActionResult(game, GetBoardDTOFromGraph(game));
            return result;
        }

        public HiveActionResult Put(PutInsectRequest request)
        {
            _moveValidator.ValidatePut(request);

            Game? game = _gameRepository.GetByPlayerId(request.PlayerId);
            var board = game.Board;

            var insect = _insectFactory.CreateInsect(request.InsectToPut, game.CurrentColorMove);
            var where = board.GetVertexByCoord(request.WhereToPut);

            if (where == null)
                throw new Exception("Vertex not found");

            where.AddInsectToStack(insect);
            board.AddEmptyVerticesAround(where);

            game.AfterActionMade();

            var result = new HiveActionResult(game, GetBoardDTOFromGraph(game));
            return result;
        }

        public HiveActionResult PutFirstInsect(PutFirstInsectRequest request)
        {
            _moveValidator.ValidatePutFirstInsect(request);

            Game? game = _gameRepository.GetByPlayerId(request.PlayerId);
            var board = game.Board;

            Vertex vertex;
            Insect insect = _insectFactory.CreateInsect(request.InsectToPut, game.CurrentColorMove);

            if (board.NotEmptyVertices.Count == 0)
                vertex = new Vertex(0, 0, insect);
            else if (board.NotEmptyVertices.Count == 1)
                vertex = new Vertex(1, 0, insect);
            else
                throw new Exception("It's not first insect");

            board.AddVertex(vertex);
            board.AddEmptyVerticesAround(vertex);

            game.AfterActionMade();
            var result = new HiveActionResult(game, GetBoardDTOFromGraph(game));
            return result;
        }

        private BoardDTO GetBoardDTOFromGraph(Game game, PlayerColor color = PlayerColor.White)
        {
            var verticesDTO = game.Board.CreateBoardDTO(color);
            return verticesDTO;
        }
    }
}