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

        }

        //can move on adjacent field
        public override IList<Vertex> GetAllEmptyVertices(Vertex moveFrom, IList<Vertex> vertices)
        {
            //if is surrouded by at least 5 insects, can't move
            if(moveFrom.GetAdjacentVerticesList(vertices).Count >=5)
                return new List<Vertex>();

            vertices = base.GetAllEmptyVertices(moveFrom, vertices);
            
            vertices = vertices.Intersect(moveFrom.GetAdjacentVerticesList(vertices)).ToList();

            return vertices;
        }
    }
}
