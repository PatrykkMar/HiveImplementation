using HiveGame.BusinessLogic.Models.Graph;
using HiveGame.BusinessLogic.Models.Requests;

namespace HiveGame.BusinessLogic.Services
{
    public interface IHiveGameService
    {
        public bool Move(MoveRequest request);

        public bool Put(PutRequest request);

        public bool PutFirstInsect(PutFirstInsectRequest request);
    }

    public class HiveGameService : IHiveGameService
    {
        private readonly IHiveGraph _graph;
        public HiveGameService(IHiveGraph graph) 
        {
            _graph = graph;
        }

        public bool Move(MoveRequest request)
        {
            var vertexFrom = _graph.GetVertexByCoord(request.MoveFrom);
            var vertexTo = _graph.GetVertexByCoord(request.MoveTo);
            return _graph.Move(vertexFrom, vertexTo, null);
        }

        public bool Put(PutRequest request)
        {
            var vertexFrom = _graph.GetVertexByCoord(request.WhereToPut);
            return _graph.Put(request.InsectToPut, vertexFrom, null);
        }

        public bool PutFirstInsect(PutFirstInsectRequest request)
        {
            return _graph.PutFirstInsect(request.InsectToPut, null);
        }
    }
}