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

            List<Vertex> hexesToMoveFromfreeHexes = freeHexesAround
                .SelectMany(x => GetVerticesByBFS(moveFrom, board, limit: 1))
                .Distinct()
                .ToList();

            vertices = vertices.Intersect(hexesToMoveFromfreeHexes).ToList();

            return vertices;
        }
    }
}
