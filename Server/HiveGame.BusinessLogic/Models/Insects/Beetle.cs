using HiveGame.BusinessLogic.Models.Extensions;
using HiveGame.BusinessLogic.Models.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveGame.BusinessLogic.Models.Insects
{
    public class Beetle : Insect
    {
        public Beetle(PlayerColor color)
        {
            Type = InsectType.Beetle;
            PlayerColor = color;
        }

        public override IList<Vertex> GetAvailableVertices(Vertex moveFrom, HiveBoard board)
        {
            List<Vertex> vertices = BasicCheck(moveFrom, board, onlyEmpty: false);

            var freeHexesAround = CheckNotSurroundedFields(moveFrom, board)
                .Union(board.GetAdjacentVerticesByCoordList(moveFrom).Where(x => !x.IsEmpty)).ToList(); //beetle can move on insect

            if (freeHexesAround.Count == 0)
                return new List<Vertex>();

            vertices = vertices.Intersect(freeHexesAround).ToList();

            return vertices;
        }
    }
}
