using HiveGame.BusinessLogic.Models.Graph;
using QuickGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveGame.BusinessLogic.Models.Insects
{
    public class Beetle : Insect
    {
        public Beetle()
        {
            Type = InsectType.Beetle;
        }

        public override IList<Vertex> GetAvailableVertices(Vertex moveFrom, HiveGraph graph)
        {
            throw new NotImplementedException();
        }
    }
}
