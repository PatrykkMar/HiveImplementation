using HiveGame.BusinessLogic.Models.Board;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HiveGame.BusinessLogic.Models.Extensions;
using static HiveGame.BusinessLogic.Models.Board.DirectionConsts;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

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
        [BsonRepresentation(BsonType.String)]
        public InsectType Type { get; set; }
        [BsonRepresentation(BsonType.String)]
        public PlayerColor PlayerColor { get; set; }

        /// <summary>
        /// Check if insect is moved and if insect doesn't move on empty vertex which would be deleted after move. All vertices meeting this condition are removed
        /// </summary>
        /// <param name="moveFrom"></param>
        /// <param name="board"></param>
        /// <returns></returns>
        public List<Vertex> BasicCheck(Vertex moveFrom, HiveBoard board, out string? whyMoveImpossible, bool onlyEmpty = true)
        {
            whyMoveImpossible = null;
            //hive can't divide to two separated hives
            if (BrokeHive(moveFrom, board))
            {
                whyMoveImpossible = "Moving this insect would break the hive";
                return new List<Vertex>();
            }


            //cannot move insects until queen is not in the game
            if (QueenNotSet(board))
            {
                whyMoveImpossible = "It's 4th turn, you have to put the queen now";
                return new List<Vertex>();
            }

            //most insects can move only on empty vertices
            List<Vertex> vertices = onlyEmpty ? board.EmptyVertices : board.Vertices;


            //cannot move on empty vertex which would be deleted after move
            if(moveFrom.InsectStack.Count==1)
            {
                var verticesToRemove = board.GetAdjacentVerticesByCoordList(moveFrom)
                    .Where(x => board
                    .GetAdjacentVerticesByCoordList(x)
                    .Where(x=>!x.IsEmpty)
                    .Count() == 1); 

                vertices = vertices
                    .Except(verticesToRemove)
                    .ToList();
            }

            //cannot stay during move
            vertices.Remove(moveFrom);

            return vertices;
        }

        public bool BrokeHive(Vertex moveFrom, HiveBoard board)
        {
            if (moveFrom.InsectStack.Count > 1) //if there is a stack of insects, hive won't be broke
                return false;


            var vertices = board.NotEmptyVertices.Where(x=>x != moveFrom).ToList();

            if(vertices.Count == 0) 
                return false;


            var result = new List<Vertex>();
            var visited = new HashSet<Vertex>();
            var queue = new Queue<Vertex>();

            queue.Enqueue(vertices[0]);
            visited.Add(vertices[0]);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                if (!current.IsEmpty)
                {
                    result.Add(current);
                }

                var adjacent = board.GetAdjacentVerticesByCoordList(current).Where(x => !x.IsEmpty && x != moveFrom);

                foreach (var edge in adjacent)
                {
                    if (!visited.Contains(edge))
                    {
                        visited.Add(edge);
                        queue.Enqueue(edge);
                    }
                }
            }

            return vertices.Count != result.Count;
        }

        public bool QueenNotSet(HiveBoard board)
        {
            var queen = board.NotEmptyVertices.SelectMany(x=>x.InsectStack).FirstOrDefault(x => x.PlayerColor == PlayerColor && x.Type == InsectType.Queen);
            return queen == null;
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
                    y.X == NeighborOffsetsDict[x].X + moveFrom.X && 
                    y.Y == NeighborOffsetsDict[x].Y + moveFrom.Y
                    )
                );

            Direction currentDirection = 0;

            var freePoints = new List<Point2D>();

            for(int i=0;i<6/*number of directions*/;i++)
            {

                var current = board.GetVertexFromVertexAtDirection(moveFrom, currentDirection);
                var next = board.GetVertexFromVertexAtDirection(moveFrom, NextDirection(currentDirection));

                if ((current == null || current.IsEmpty) && (next == null || next.IsEmpty))
                {
                    freePoints.Add(moveFrom.Coords.Add(NeighborOffsetsDict[currentDirection].To2D()));
                    freePoints.Add(moveFrom.Coords.Add(NeighborOffsetsDict[NextDirection(currentDirection)].To2D()));
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

        public abstract InsectValidationResult GetAvailableVertices(Vertex moveFrom, HiveBoard board);

        public List<Vertex> GetVerticesByBFS(Vertex moveFrom, HiveBoard board, int? limit = null, int? onlyDistance = null)
        {
            var result = new List<Vertex>();
            var visited = new HashSet<Vertex>();
            var queue = new Queue<(Vertex vertex, int distance)>();

            queue.Enqueue((moveFrom, 0));
            visited.Add(moveFrom);

            while (queue.Count > 0)
            {
                var (current, distance) = queue.Dequeue();

                if (current.IsEmpty && (!onlyDistance.HasValue || distance == onlyDistance.Value))
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
        Nothing=-1, Queen=0, Ant=1, Spider=2, Grasshopper=3, Beetle=4
    }
}
