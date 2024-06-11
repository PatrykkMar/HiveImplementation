using AutoMapper;
using HiveGame.BusinessLogic.Models.Graph;
using HiveGame.BusinessLogic.Models.Insects;
using HiveGame.BusinessLogic.Models.Requests;
using static Parser.Token;

namespace HiveGame.BusinessLogic.Services
{
    public interface IHiveGameService
    {
        public IList<VertexDTO> Move(MoveRequest request);

        public IList<VertexDTO> Put(PutRequest request);

        public IList<VertexDTO> PutFirstInsect(PutFirstInsectRequest request);
        IList<VertexDTO> GetTestGrid();
        string GetTestGridPrint();
    }

    public class HiveGameService : IHiveGameService
    {
        private readonly HiveGraph _graph;
        private readonly IMapper _mapper;
        public HiveGameService(HiveGraph graph, IMapper mapper) 
        {
            _graph = graph;
            _mapper = mapper;
        }

        public IList<VertexDTO> GetTestGrid()
        {
            _graph.PutFirstInsect(InsectType.Ant, null);
            _graph.Put(InsectType.Queen, _graph.GetVertexByCoord(1, -1, 0), null);
            //_graph.Move(_graph.GetVertexByCoord(0, 0, 0), _graph.GetVertexByCoord(2, -2 ,0), null);
            var dtos = GetVerticesDTOFromGraph();
            return dtos;
        }

        public string GetTestGridPrint()
        {
            _graph.PutFirstInsect(InsectType.Ant, null);
            //_graph.Put(InsectType.Queen, _graph.GetVertexByCoord(1, -1, 0), null);
            //_graph.Move(_graph.GetVertexByCoord(0, 0, 0), _graph.GetVertexByCoord(2, -2 ,0), null);
            var dtos = "";
            dtos += "Vertices\n";
            foreach (var vertex in _graph.Vertices)
            {
                dtos += vertex.PrintVertex() + "\n";
            }

            // Wypisujemy krawędzie
            dtos += "Edges:\n";
            foreach (var edge in _graph.Edges)
            {
                dtos += $"{((edge.Source.PrintVertex()))} -> {edge.Target.PrintVertex()} {edge.Direction}\n";
            }
            return dtos;
        }

        public IList<VertexDTO> Move(MoveRequest request)
        {
            var vertexFrom = _graph.GetVertexByCoord(request.MoveFrom);
            var vertexTo = _graph.GetVertexByCoord(request.MoveTo);
            _graph.Move(vertexFrom, vertexTo, null);
            return GetVerticesDTOFromGraph();
        }

        public IList<VertexDTO> Put(PutRequest request)
        {
            var vertexFrom = _graph.GetVertexByCoord(request.WhereToPut);
            _graph.Put(request.InsectToPut, vertexFrom, null);
            return GetVerticesDTOFromGraph();
        }

        public IList<VertexDTO> PutFirstInsect(PutFirstInsectRequest request)
        {
            _graph.PutFirstInsect(request.InsectToPut, null);
            return GetVerticesDTOFromGraph();
        }

        private IList<VertexDTO> GetVerticesDTOFromGraph()
        {
            var verticesDTO = _mapper.Map<IList<VertexDTO>>(_graph.Vertices);
            return verticesDTO;
        }
    }
}