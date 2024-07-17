using AutoMapper;
using HiveGame.BusinessLogic.Managers;
using HiveGame.BusinessLogic.Models.Game.Graph;
using HiveGame.BusinessLogic.Models.Graph;
using HiveGame.BusinessLogic.Models.Insects;
using HiveGame.BusinessLogic.Models.Requests;

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
        private readonly HiveBoard _board;
        private readonly IMapper _mapper;
        public HiveGameService(HiveBoard board, IMapper mapper) 
        {
            _board = board;
            _mapper = mapper;
        }

        public IList<VertexDTO> GetTestGrid()
        {
            _board.PutFirstInsect(InsectType.Ant, null);
            _board.Put(InsectType.Queen, _board.GetVertexByCoord(1, -1, 0), null);
            //_board.Move(_board.GetVertexByCoord(0, 0, 0), _board.GetVertexByCoord(2, -2 ,0), null);
            var dtos = GetVerticesDTOFromGraph();
            return dtos;
        }

        public string GetTestGridPrint()
        {
            _board.PutFirstInsect(InsectType.Ant, null);
            //_board.Put(InsectType.Queen, _board.GetVertexByCoord(1, -1, 0), null);
            //_board.Move(_board.GetVertexByCoord(0, 0, 0), _board.GetVertexByCoord(2, -2 ,0), null);
            var dtos = "";
            dtos += "Vertices\n";
            foreach (var vertex in _board.Vertices)
            {
                dtos += vertex.PrintVertex() + "\n";
            }
            return dtos;
        }

        public IList<VertexDTO> Move(MoveInsectRequest request)
        {
            var vertexFrom = _board.GetVertexByCoord(request.MoveFrom);
            var vertexTo = _board.GetVertexByCoord(request.MoveTo);
            _board.Move(vertexFrom, vertexTo, null);
            return GetVerticesDTOFromGraph();
        }

        public IList<VertexDTO> Put(PutInsectRequest request)
        {
            var vertexFrom = _board.GetVertexByCoord(request.WhereToPut);
            _board.Put(request.InsectToPut, vertexFrom, null);
            return GetVerticesDTOFromGraph();
        }

        public IList<VertexDTO> PutFirstInsect(PutFirstInsectRequest request)
        {
            _board.PutFirstInsect(request.InsectToPut, null);
            return GetVerticesDTOFromGraph();
        }

        private IList<VertexDTO> GetVerticesDTOFromGraph()
        {
            var verticesDTO = _mapper.Map<IList<VertexDTO>>(_board.Vertices);
            return verticesDTO;
        }
    }
}