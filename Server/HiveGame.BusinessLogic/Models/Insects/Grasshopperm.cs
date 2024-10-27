using HiveGame.BusinessLogic.Models.Extensions;
using HiveGame.BusinessLogic.Models.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HiveGame.BusinessLogic.Models.Graph.DirectionConsts;

namespace HiveGame.BusinessLogic.Models.Insects
{
    public class Grasshopperm : Insect
    {
        public Grasshopperm(PlayerColor color)
        {
            Type= InsectType.Grasshopperm;
            PlayerColor = color;
        }

        public override IList<Vertex> GetAvailableVertices(Vertex moveFrom, HiveBoard board)
        {
            var vertices = BasicCheck(moveFrom, board);
            var possibleMoves = GetPossibleMovesForGrasshopperm(moveFrom, board);

            return vertices
                .Where(x => possibleMoves.Any(y => y.Equals(x)))
                .Distinct()
                .ToList();
        }

        public List<Vertex> GetPossibleMovesForGrasshopperm(Vertex moveFrom, HiveBoard board)
        {
            var possibleMoves = new List<Vertex>();

            foreach ( var direction in Enum.GetValues<Direction>()) 
            {
                if (direction == Direction.Up || direction == Direction.Down)
                    continue;

                var currentPoint = moveFrom.Coords;

                var (dx, dy) = NeighborOffsetsDict[direction].To2D();

                while(!board.GetVertexByCoord(currentPoint).IsEmpty)
                {
                    currentPoint = (currentPoint.x + dx, currentPoint.y + dy);
                }

                possibleMoves.Add(board.GetVertexByCoord(currentPoint));
            }

            var verticesToRemove = board.GetAdjacentVerticesByCoordList(moveFrom);

            possibleMoves = possibleMoves.Except(verticesToRemove).ToList();

            return possibleMoves;
        }
    }
}
