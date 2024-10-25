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
            List<Vertex> vertices = BasicCheck(moveFrom, board);

            Vertex vertexUnder = board.GetVertexByCoord((moveFrom.Coords.x, moveFrom.Coords.y, 0));

            List<Vertex> hexesToMove = board.GetAdjacentVerticesByCoordList(vertexUnder);

            for(int i = 0; i < hexesToMove.Count; i++)
            {
                while (!hexesToMove[i].IsEmpty)
                {
                    hexesToMove[i] = board.GetVertexByCoord(hexesToMove[i].Coords.Add((0, 0, 1)));
                }
            }

            vertices = vertices.Intersect(hexesToMove).ToList();

            return vertices;
        }
    }
}
