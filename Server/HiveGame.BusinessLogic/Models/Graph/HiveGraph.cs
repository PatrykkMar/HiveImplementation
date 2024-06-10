using HiveGame.BusinessLogic.Factories;
using HiveGame.BusinessLogic.Models;
using HiveGame.BusinessLogic.Models.Insects;
using HiveGame.Core.Models;
using QuickGraph;
using QuickGraph.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using static HiveGame.BusinessLogic.Models.Graph.DirectionConsts;

namespace HiveGame.BusinessLogic.Models.Graph
{
    public class HiveGraph : AdjacencyGraph<Vertex, DirectedEdge<Vertex>>
    {
        private readonly IInsectFactory _factory;
        public HiveGraph(IInsectFactory factory) 
        {
            _factory = factory;
        }
        public bool Move(Vertex moveFrom, Vertex moveTo, Player player)
        {
            if (moveFrom.IsEmpty)
                throw new ArgumentException("There is not insect on the 'moveFrom' vertex");

            if (!moveTo.IsEmpty)
                throw new ArgumentException("'emptyVertex' is not empty");

            if (GetVertexByCoord(moveFrom.X, moveFrom.Y, moveFrom.Z + 1)?.IsEmpty == false)
                throw new ArgumentException("Insect cannot move because there is another insect above him");


            RemoveAllEmptyUnconnectedVerticesAround(moveFrom);

            var insect = moveFrom.CurrentInsect;
            moveTo.CurrentInsect = insect;
            moveFrom.CurrentInsect = null;
            RemoveAllEmptyUnconnectedVerticesAround(moveFrom);

            AddEmptyVerticesAround(moveTo);
            return true;
        }

        public bool Put(InsectType? insectType, Vertex where, Player player)
        {
            if(insectType == null) 
                throw new ArgumentNullException("Vertex cannot be null");
            if (IsVerticesEmpty)
                throw new Exception("Cannot put insect on the empty graph");

            //adjacency rule

            //check if "where" vertex is empty

            var insect = _factory.CreateInsect(insectType.Value);

            where.CurrentInsect = insect;
            AddEmptyVerticesAround(where);
            return true;
        }

        public bool PutFirstInsect(InsectType insectType, Player player)
        {
            Vertex vertex;
            Insect insect = _factory.CreateInsect(insectType);

            if (VertexCount == 0)
                vertex = new Vertex(0, 0, 0, insect);
            else
                throw new Exception("First insect is put");
            AddVertex(vertex);
            AddEmptyVerticesAround(vertex);
            return true;
        }

        private void AddEmptyVerticesAround(Vertex vertex, bool ignoreDown = true)
        {
            var adjacentEdges = OutEdges(vertex);
            foreach(var direction in (Direction[])Enum.GetValues(typeof(Direction)))
            {
                if(ignoreDown && direction == Direction.Down) 
                    continue;

                if (!adjacentEdges.Any(x => x.Direction == direction))
                {
                    var emptyVertex = new Vertex(vertex, direction);
                    AddVertex(emptyVertex);
                    AddEdge(new DirectedEdge<Vertex>(vertex, emptyVertex, direction));
                    AddEdge(new DirectedEdge<Vertex>(emptyVertex, vertex, OppositeDirection(direction)));
                }
            }
        }

        private bool IsEndgameConditionMet()
        {
            //check if some player's queen is surronded in every side
            throw new NotImplementedException();
        }

        private void RemoveAllEmptyUnconnectedVerticesAround(Vertex vertex)
        {
            var verticesToDelete = OutEdges(vertex)
                .Select(x => x.Target)
                .Where(y => y.IsEmpty && OutEdges(y).Select(z => z.Target).Where(a => !a.IsEmpty).Count() == 0).ToList();

            foreach(var vertice in verticesToDelete)
                RemoveVertex(vertice);
        }

        public Vertex? GetVertexByCoord(long x, long y, long z)
        {
            return Vertices.FirstOrDefault(a => a.X == x && a.Y == y && a.Z == z);
        }

        public Vertex? GetVertexByCoord(Point point)
        {
            return GetVertexByCoord(point.X, point.Y, point.Z);
        }


        public Dictionary<Direction, Vertex> GetAdjacentVerticesDict(Vertex vertex, bool ignoreDown = true)
        {
            var dict = new Dictionary<Direction, Vertex>();

            var topLeft = Vertices.FirstOrDefault(x => x.X == vertex.X && x.Y == vertex.Y + 1 && x.Z == vertex.Z);
            if (topLeft != null) dict.Add(Direction.TopLeft, topLeft);

            var topRight = Vertices.FirstOrDefault(x => x.X == vertex.X + 1 && x.Y == vertex.Y + 1 && x.Z == vertex.Z);
            if (topRight != null) dict.Add(Direction.TopRight, topRight);

            var left = Vertices.FirstOrDefault(x => x.X == vertex.X - 1 && x.Y == vertex.Y && x.Z == vertex.Z);
            if (left != null) dict.Add(Direction.Left, left);

            var right = Vertices.FirstOrDefault(x => x.X == vertex.X + 1 && x.Y == vertex.Y && x.Z == vertex.Z);
            if (right != null) dict.Add(Direction.Right, right);

            var bottomLeft = Vertices.FirstOrDefault(x => x.X == vertex.X && x.Y == vertex.Y - 1 && x.Z == vertex.Z);
            if (bottomLeft != null) dict.Add(Direction.BottomLeft, bottomLeft);

            var bottomRight = Vertices.FirstOrDefault(x => x.X == vertex.X - 1 && x.Y == vertex.Y - 1 && x.Z == vertex.Z);
            if (bottomRight != null) dict.Add(Direction.BottomRight, bottomRight);

            var up = Vertices.FirstOrDefault(x => x.X == vertex.X && x.Y == vertex.Y && x.Z == vertex.Z + 1);
            if (up != null) dict.Add(Direction.Up, up);

            if(!ignoreDown)
            { 
                var down = Vertices.FirstOrDefault(x => x.X == vertex.X && x.Y == vertex.Y && x.Z == vertex.Z - 1);
                if (down != null) dict.Add(Direction.Down, down);
            }

            return dict;
        }

        public List<Vertex> GetAdjacentVerticesList(Vertex vertex)
        {
            return GetAdjacentVerticesDict(vertex).Values.ToList();
        }
    }
}
