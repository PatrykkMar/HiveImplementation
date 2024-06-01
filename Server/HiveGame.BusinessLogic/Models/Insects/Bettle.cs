using HiveGame.BusinessLogic.Models.Graph;
using QuickGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveGame.BusinessLogic.Models.Insects
{
    public class Bettle : Insect
    {
        public Bettle()
        {

        }

        public override IList<Vertex> GetAllEmptyVertices(Vertex moveFrom, IList<Vertex> vertices)
        {
            //TODO
            return vertices;
        }
    }
}
