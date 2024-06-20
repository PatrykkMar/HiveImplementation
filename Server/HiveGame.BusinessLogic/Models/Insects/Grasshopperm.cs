using HiveGame.BusinessLogic.Models.Game.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveGame.BusinessLogic.Models.Insects
{
    public class Grasshopperm : Insect
    {
        public Grasshopperm()
        {
            Type= InsectType.Grasshopperm;
        }

        //can move on adjacent field
        public IList<Vertex> BasicCheck(Vertex moveFrom, HiveBoard board)
        {
            //TODO
            return null;
        }

        public override IList<Vertex> GetAvailableVertices(Vertex moveFrom, HiveBoard board)
        {
            throw new NotImplementedException();
        }
    }
}
