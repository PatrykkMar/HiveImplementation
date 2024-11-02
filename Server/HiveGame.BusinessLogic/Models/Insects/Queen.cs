using HiveGame.BusinessLogic.Models.Board;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveGame.BusinessLogic.Models.Insects
{
    public class Queen : Insect
    {
        public Queen(PlayerColor color)
        {
            Type = InsectType.Queen;
            PlayerColor = color;
        }

        public override List<Vertex> GetAvailableVertices(Vertex moveFrom, HiveBoard board)
        {
            List<Vertex> vertices = BasicCheck(moveFrom, board);

            var freeHexesAround = CheckNotSurroundedFields(moveFrom, board);

            if (freeHexesAround.Count == 0)
                return new List<Vertex>();

            vertices = vertices.Intersect(freeHexesAround).ToList();

            return vertices;
        }
    }
}
