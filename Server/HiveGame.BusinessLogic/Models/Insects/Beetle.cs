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
            //TODO: To change
            List<Vertex> vertices = BasicCheck(moveFrom, board);

            var freeHexesAround = CheckNotSurroundedFields(moveFrom, board);

            if (freeHexesAround.Count == 0)
                return new List<Vertex>();

            List<Vertex> hexesToMoveFromfreeHexes = freeHexesAround
                .SelectMany(x => GetVerticesByBFS(moveFrom, board, 1))
                .Distinct()
                .ToList();

            vertices = vertices.Where(x => hexesToMoveFromfreeHexes.Any(y => y.Equals(x))).ToList();

            return vertices;
        }
    }
}
