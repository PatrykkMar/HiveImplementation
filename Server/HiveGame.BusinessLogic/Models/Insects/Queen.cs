using HiveGame.BusinessLogic.Models.Game.Graph;
using HiveGame.BusinessLogic.Models.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveGame.BusinessLogic.Models.Insects
{
    public class Queen : Insect
    {
        public Queen()
        {
            Type = InsectType.Queen;
        }

        public override IList<Vertex> GetAvailableVertices(Vertex moveFrom, HiveBoard board)
        {
            throw new NotImplementedException();
        }
    }
}
