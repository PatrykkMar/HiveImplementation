using AutoMapper;
using HiveGame.BusinessLogic.Models.Game;
using HiveGame.BusinessLogic.Models.Game.Graph;
using HiveGame.BusinessLogic.Models.Graph;
using HiveGame.BusinessLogic.Models.Insects;
using HiveGame.BusinessLogic.Models.Requests;
using HiveGame.BusinessLogic.Repositories;

namespace HiveGame.BusinessLogic.Services
{
    public interface IHiveGameService
    {
        public IList<VertexDTO> Move(MoveInsectRequest request);

        public HiveActionResult Put(PutInsectRequest request);
        public HiveActionResult PutFirstInsect(PutFirstInsectRequest request);

        IList<VertexDTO> GetTestGrid();
        string GetTestGridPrint();
    }

    public class HiveGameService : IHiveGameService
    {
        private readonly IGameRepository _gameRepository;
        private readonly IMapper _mapper;
        public HiveGameService(IMapper mapper, IGameRepository gameRepository) 
        {
            _mapper = mapper;
            _gameRepository = gameRepository;
        }

        public IList<VertexDTO> GetTestGrid()
        {
            throw new NotImplementedException();
            //_board.PutFirstInsect(InsectType.Ant, null);
            //_board.Put(InsectType.Queen, _board.GetVertexByCoord(1, -1, 0), null);
            //_board.Move(_board.GetVertexByCoord(0, 0, 0), _board.GetVertexByCoord(2, -2 ,0), null);
            //var dtos = GetVerticesDTOFromGraph();
            //return dtos;
        }

        public string GetTestGridPrint()
        {
            throw new NotImplementedException();
            //_board.PutFirstInsect(InsectType.Ant, null);
            //_board.Put(InsectType.Queen, _board.GetVertexByCoord(1, -1, 0), null);
            //_board.Move(_board.GetVertexByCoord(0, 0, 0), _board.GetVertexByCoord(2, -2 ,0), null);
            //var dtos = "";
            //dtos += "Vertices\n";
            //foreach (var vertex in _board.Vertices)
            //{
            //    dtos += vertex.PrintVertex() + "\n";
            //}
            //return dtos;
        }

        public IList<VertexDTO> Move(MoveInsectRequest request)
        {
            throw new NotImplementedException();
            //var vertexFrom = _board.GetVertexByCoord(request.MoveFrom);
            //var vertexTo = _board.GetVertexByCoord(request.MoveTo);
            //_board.Move(vertexFrom, vertexTo, null);
            //return GetVerticesDTOFromGraph();
        }

        public HiveActionResult Put(PutInsectRequest request)
        {
            var game = GetGame(request.PlayerId);

            if (game == null)
                throw new Exception("Game not found");

            if (request.PlayerId != game?.GetCurrentPlayer().PlayerId)
                throw new Exception("It's not your move");

            game.Board.Put(request.InsectToPut, request.WhereToPut);

            game.GetCurrentPlayer().NumberOfMove++;

            var result = new HiveActionResult(game, GetVerticesDTOFromGraph(game));

            return result;
        }

        public HiveActionResult PutFirstInsect(PutFirstInsectRequest request)
        {
            var game = GetGame(request.PlayerId);
            if (request.PlayerId != game?.GetCurrentPlayer().PlayerId)
            {
                throw new Exception("It's not your move");
            }

            game.Board.PutFirstInsect(request.InsectToPut);
            game.AfterActionMade();
            var result = new HiveActionResult(game, GetVerticesDTOFromGraph(game));

            return result;
        }

        private List<VertexDTO> GetVerticesDTOFromGraph(Game game)
        {
            var verticesDTO = _mapper.Map<List<VertexDTO>>(game.Board.Vertices);
            return verticesDTO;
        }

        private Game? GetGame(string playerId)
        {
            return _gameRepository.GetByPlayerId(playerId);
        }
    }
}