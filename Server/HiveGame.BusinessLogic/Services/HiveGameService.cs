using AutoMapper;
using HiveGame.BusinessLogic.Managers;
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

        public IList<VertexDTO> Put(PutInsectRequest request);

        public IList<VertexDTO> PutFirstInsect(PutFirstInsectRequest request);
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
            var dtos = GetVerticesDTOFromGraph();
            return dtos;
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

        public IList<VertexDTO> Put(PutInsectRequest request)
        {
            throw new NotImplementedException();
            //var vertexFrom = _board.GetVertexByCoord(request.WhereToPut);
            //_board.Put(request.InsectToPut, vertexFrom, null);
            //return GetVerticesDTOFromGraph();
        }

        public IList<VertexDTO> PutFirstInsect(PutFirstInsectRequest request)
        {
            var game = GetGame(request.PlayerId);
            game.Board.PutFirstInsect(request.InsectToPut, null);
            return GetVerticesDTOFromGraph();
        }

        private IList<VertexDTO> GetVerticesDTOFromGraph()
        {
            throw new NotImplementedException();
            //var verticesDTO = _mapper.Map<IList<VertexDTO>>(_board.Vertices);
            //return verticesDTO;
        }

        private Game? GetGame(string playerId)
        {
            return _gameRepository.GetByPlayerId(playerId);
        }
    }
}