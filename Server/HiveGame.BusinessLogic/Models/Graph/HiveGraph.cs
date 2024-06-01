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
    public interface IHiveGraph
    {
        public bool Move(Vertex moveFrom, Vertex moveTo, Player player);
        public bool Put(InsectType? what, Vertex where, Player player);
        public bool PutFirstInsect(InsectType insect, Player player);
        public Vertex? GetVertexByCoord(long x, long y);
        Vertex? GetVertexByCoord(Point point);
        IEnumerable<Vertex> Vertices { get; }

    }

    public class HiveGraph : AdjacencyGraph<Vertex, DirectedEdge<Vertex>>, IHiveGraph
    {
        public bool Move(Vertex moveFrom, Vertex moveTo, Player player)
        {
            if (moveFrom.IsEmpty)
                throw new ArgumentException("There is not insect on the 'moveFrom' vertex");

            if (!moveTo.IsEmpty)
                throw new ArgumentException("'emptyVertex' is not empty");

            RemoveAllEmptyUnconnectedVerticesAround(moveFrom);

            var insect = moveFrom.CurrentInsect;
            moveTo.CurrentInsect = insect;
            moveFrom.CurrentInsect = null;
            RemoveAllEmptyUnconnectedVerticesAround(moveFrom);

            AddEmptyVerticesAround(moveTo);
            return true;
        }

        public bool Put(InsectType? insertType, Vertex where, Player player)
        {
            if(insertType == null) 
                throw new ArgumentNullException("Vertex cannot be null");
            if (IsVerticesEmpty)
                throw new Exception("Cannot put insect on the empty graph");

            //adjacency rule

            //check if "where" vertex is empty

            var insect = new Insect(insertType.Value);

            where.CurrentInsect = insect;
            AddEmptyVerticesAround(where);
            return true;
        }

        public bool PutFirstInsect(InsectType insectType, Player player)
        {
            Vertex vertex;
            var insect = new Insect(insectType);

            if (VertexCount == 0)
                vertex = new Vertex(0, 0, insect);
            //else if (VertexCount == 1)
            //    vertex = new Vertex(1, 0, insect);
            else
                throw new Exception("First insect is put");
            AddVertex(vertex);
            AddEmptyVerticesAround(vertex);
            return true;
        }

        private void AddEmptyVerticesAround(Vertex vertex)
        {
            var adjacentVertices = OutEdges(vertex);
            foreach(var direction in (Direction[])Enum.GetValues(typeof(Direction)))
            {
                if (!adjacentVertices.Any(x => x.Direction == direction))
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

        public Vertex? GetVertexByCoord(long x, long y)
        {
            return Vertices.FirstOrDefault(a => a.X == x && a.Y == y);
        }

        public Vertex? GetVertexByCoord(Point point)
        {
            return GetVertexByCoord(point.X, point.Y);
        }
    }
}
