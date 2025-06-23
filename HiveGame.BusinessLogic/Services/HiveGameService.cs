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
        Task<HiveActionResult> MoveAsync(MoveInsectRequest request);
        Task<HiveActionResult> PutAsync(PutInsectRequest request);
        Task<HiveActionResult> PutFirstInsectAsync(PutFirstInsectRequest request);
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

        public async Task<HiveActionResult> MoveAsync(MoveInsectRequest request)
        {
            var dbModel = await _gameRepository.GetByPlayerIdAsync(request.PlayerId);
            Game? game = _converter.FromGameDbModel(dbModel);
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

            return await AfterMoveActionsAsync(game);
        }

        public async Task<HiveActionResult> PutAsync(PutInsectRequest request)
        {
            var dbModel = await _gameRepository.GetByPlayerIdAsync(request.PlayerId);
            Game? game = _converter.FromGameDbModel(dbModel);
            _moveValidator.ValidatePut(request, game);

            var board = game.Board;

            var insect = _insectFactory.CreateInsect(request.InsectToPut, game.CurrentColorMove);
            var where = board.GetVertexByCoord(request.WhereToPut);

            if (where == null)
                throw new Exception("Vertex not found");

            where.AddInsectToStack(insect);
            board.AddEmptyVerticesAround(where);

            return await AfterMoveActionsAsync(game);
        }

        public async Task<HiveActionResult> PutFirstInsectAsync(PutFirstInsectRequest request)
        {
            var dbModel = await _gameRepository.GetByPlayerIdAsync(request.PlayerId);
            Game? game = _converter.FromGameDbModel(dbModel);

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

            return await AfterMoveActionsAsync(game);
        }

        private async Task<HiveActionResult> AfterMoveActionsAsync(Game game)
        {
            game.AfterActionMade();

            var result = new HiveActionResult(game);
            result.GameOver = game.GameOverConditionMet();

            var players = game.Players;
            foreach (var player in players)
            {
                if (!result.GameOver)
                {
                    if (player.PlayerState == ClientState.InGamePlayerFirstMove || player.PlayerState == ClientState.InGamePlayerMove)
                    {
                        player.PlayerState = ClientState.InGameOpponentMove;
                    }
                    else if (player.PlayerState == ClientState.InGameOpponentMove)
                    {
                        player.PlayerState = game.Board.FirstMoves ? ClientState.InGamePlayerFirstMove : ClientState.InGamePlayerMove;
                    }

                    await _gameRepository.UpdateAsync(game.Id, _converter.ToGameDbModel(game));
                }
                else
                {
                    player.PlayerState = ClientState.GameOver;
                    await _gameRepository.RemoveAsync(game.Id);
                }
            }

            return result;
        }
    }
}