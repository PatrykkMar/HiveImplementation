using HiveGame.BusinessLogic.Models.Graph;
using QuickGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveGame.BusinessLogic.Models.Insects
{
    public class Spider : Insect
    {
        public Spider()
        {
            Type = InsectType.Spider;
        }

        //can move on adjacent field
        public override IList<Vertex> GetAvailableVertices(Vertex moveFrom, HiveGraph graph)
        {
            //if is surrouded by at least 5 insects, can't move
            if (CheckIfSurrounded(moveFrom, graph))
                return new List<Vertex>();

            var vertices = BasicCheck(moveFrom, graph);
            
            vertices = vertices.Intersect(graph.GetAdjacentVerticesByCoordList(moveFrom)).ToList();

            return vertices;
        }
    }
}
