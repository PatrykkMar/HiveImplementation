using HiveGame.BusinessLogic.Models.Game.Graph;
using HiveGame.BusinessLogic.Models.Graph;
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
        public override IList<Vertex> GetAvailableVertices(Vertex moveFrom, HiveBoard board)
        {
            //if is surrouded by at least 5 insects, can't move
            if (CheckIfSurrounded(moveFrom, board))
                return new List<Vertex>();

            var vertices = BasicCheck(moveFrom, board);
            
            vertices = vertices.Intersect(board.GetAdjacentVerticesByCoordList(moveFrom)).ToList();

            return vertices;
        }
    }
}
