using HiveGame.BusinessLogic.Factories;
using HiveGame.BusinessLogic.Models.Board;
using HiveGame.BusinessLogic.Models.Insects;
using HiveGame.BusinessLogic.Models.Requests;
using HiveGame.BusinessLogic.Models;
using HiveGame.Core.Models;
using HiveGame.DataAccess.Repositories;
using HiveGame.BusinessLogic.Utils;
using HiveGame.BusinessLogic.Repositories;
using System.Numerics;

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
        private readonly IMatchmakingRepository _matchmakingRepository;
        private readonly IInsectFactory _insectFactory;
        private readonly IHiveMoveValidator _moveValidator;
        private readonly IGameConverter _converter;

        public HiveGameService(IInsectFactory insectFactory, IGameRepository gameRepository, IHiveMoveValidator hiveMoveValidator, 
            IGameConverter converter, IMatchmakingRepository matchmakingRepository)
        {
            _gameRepository = gameRepository;
            _insectFactory = insectFactory;
            _moveValidator = hiveMoveValidator;
            _converter = converter;
            _matchmakingRepository = matchmakingRepository;
        }

        public HiveActionResult Move(MoveInsectRequest request)
        {
            Game? game = _converter.FromGameDbModel(_gameRepository.GetByPlayerId(request.PlayerId));
            _moveValidator.ValidateMove(request, game);

            var board = game.Board;

            var moveFromVertex = board.GetVertexByCoord(request.MoveFrom);
            var moveToVertex = board.GetVertexByCoord(request.MoveTo);
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

            return AfterMoveActions(game);
        }

        public HiveActionResult Put(PutInsectRequest request)
        {
            Game? game = _converter.FromGameDbModel(_gameRepository.GetByPlayerId(request.PlayerId));
            _moveValidator.ValidatePut(request, game);

            var board = game.Board;

            var insect = _insectFactory.CreateInsect(request.InsectToPut, game.CurrentColorMove);
            var where = board.GetVertexByCoord(request.WhereToPut);

            if (where == null)
                throw new Exception("Vertex not found");

            where.AddInsectToStack(insect);
            board.AddEmptyVerticesAround(where);

            return AfterMoveActions(game);
        }

        public HiveActionResult PutFirstInsect(PutFirstInsectRequest request)
        {
            Game? game = _converter.FromGameDbModel(_gameRepository.GetByPlayerId(request.PlayerId));

            _moveValidator.ValidatePutFirstInsect(request, game);

            var board = game.Board;

            IVertex vertex;
            Insect insect = _insectFactory.CreateInsect(request.InsectToPut, game.CurrentColorMove);

            if (board.NotEmptyVertices.Count == 0)
                vertex = new Vertex(0, 0, insect);
            else if (board.NotEmptyVertices.Count == 1)
                vertex = new Vertex(1, 0, insect);
            else
                throw new Exception("It's not first insect");

            board.AddVertex(vertex);
            board.AddEmptyVerticesAround(vertex);

            return AfterMoveActions(game);
        }

        public HiveActionResult AfterMoveActions(Game game)
        {
            game.AfterActionMade();

            var result = new HiveActionResult(game);

            var players = game.Players;
            foreach ( var player in players)
            {
                if (player.PlayerState == ClientState.InGamePlayerFirstMove || player.PlayerState == ClientState.InGamePlayerMove)
                {
                    player.PlayerState = ClientState.InGameOpponentMove;
                }
                else if(player.PlayerState == ClientState.InGameOpponentMove)
                {
                    player.PlayerState = game.Board.FirstMoves ? ClientState.InGamePlayerFirstMove : ClientState.InGamePlayerMove;
                }
            }

            result.GameOver = game.CheckGameOverCondition();
            _gameRepository.Update(game.Id, _converter.ToGameDbModel(game));


            return result;
        }
    }
}