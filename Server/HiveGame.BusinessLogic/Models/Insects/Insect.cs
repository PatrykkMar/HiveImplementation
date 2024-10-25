using HiveGame.BusinessLogic.Models.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HiveGame.BusinessLogic.Models.Extensions;
using static HiveGame.BusinessLogic.Models.Graph.DirectionConsts;

namespace HiveGame.BusinessLogic.Models.Insects
{
    public abstract class Insect
    {
        public Insect()
        {
        }

        public Insect(InsectType type, PlayerColor color)
        {
            Type = type;
            PlayerColor = color;
        }

        public InsectType Type { get; set; }
        public PlayerColor PlayerColor { get; set; }

        /// <summary>
        /// Check if insect is moved and if insect doesn't move on empty vertex which would be deleted after move. All vertices meeting this condition are removed
        /// </summary>
        /// <param name="moveFrom"></param>
        /// <param name="board"></param>
        /// <returns></returns>
        public List<Vertex> BasicCheck(Vertex moveFrom, HiveBoard board)
        {
            var vertices = board.Vertices.ToList();

            var verticesToRemove = board.GetAdjacentVerticesByCoordList(moveFrom)
                .Where(x => board.GetAdjacentVerticesByCoordList(x).Count == 1); //cannot move on empty vertex which would be deleted after move

            foreach(var v in verticesToRemove)
                vertices.Remove(v);

            vertices.Remove(moveFrom); //cannot stay during move

            return vertices;
        }

        public List<Vertex> CheckNotSurroundedFields(Vertex moveFrom, HiveBoard board)
        {
            var surroundings = board
                .GetAdjacentVerticesByCoordList(moveFrom)
                .Where(x => !x.IsEmpty)
                .ToList();

            var surroudingDirections = Enum.GetValues<Direction>()
                .Where(x => x != Direction.Up && x != Direction.Down)
                .Where(x =>
                    surroundings.Any(y => 
                    y.X == NeighborOffsetsDict[x].dx + moveFrom.X && 
                    y.Y == NeighborOffsetsDict[x].dy + moveFrom.Y && 
                    y.Z == NeighborOffsetsDict[x].dz + moveFrom.Z)
                );

            Direction currentDirection = 0;

            var freePoints = new List<(int, int, int)>();

            for(int i=0;i<6/*number of directions*/;i++)
            {

                var current = board.GetVertexFromVertexAtDirection(moveFrom, currentDirection);
                var next = board.GetVertexFromVertexAtDirection(moveFrom, NextDirection(currentDirection));

                if ((current == null || current.IsEmpty) && (next == null || next.IsEmpty))
                {
                    freePoints.Add(moveFrom.Coords.Add(NeighborOffsetsDict[currentDirection]));
                    freePoints.Add(moveFrom.Coords.Add(NeighborOffsetsDict[NextDirection(currentDirection)]));
                }

                currentDirection = NextDirection(currentDirection);
            }

            List<Vertex> freeVertices = freePoints
                .Distinct()
                .Select(x => board.GetVertexByCoord(x))
                .Where(x => x != null)
                .ToList();

            return freeVertices;
        }

        public bool CheckIfSurrounded(Vertex moveFrom, HiveBoard board)
        {
            return CheckNotSurroundedFields(moveFrom, board).Count == 0;
        }

        public abstract IList<Vertex> GetAvailableVertices(Vertex moveFrom, HiveBoard board);

        public List<Vertex> GetVerticesByBFS(Vertex moveFrom, HiveBoard board, int? limit = null)
        {
            var result = new List<Vertex>();
            var visited = new HashSet<Vertex>();
            var queue = new Queue<(Vertex vertex, int distance)>();

            queue.Enqueue((moveFrom, 0));
            visited.Add(moveFrom);

            while (queue.Count > 0)
            {
                var (current, distance) = queue.Dequeue();

                if (current.IsEmpty)
                {
                    result.Add(current);
                }

                if (limit.HasValue && distance >= limit.Value)
                {
                    continue;
                }

                var adjacent = CheckNotSurroundedFields(current, board);

                foreach (var edge in adjacent)
                {
                    if (!visited.Contains(edge))
                    {
                        visited.Add(edge);
                        queue.Enqueue((edge, distance + 1));
                    }
                }
            }

            return result;
        }
    }


    public enum InsectType
    {
        Nothing=-1, Queen=0, Ant=1, Spider=2, Grasshopperm=3, Beetle=4
    }
}
