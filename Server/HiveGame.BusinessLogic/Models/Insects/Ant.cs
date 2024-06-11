using HiveGame.BusinessLogic.Models.Graph;
using QuickGraph;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveGame.BusinessLogic.Models.Insects
{
    public class Ant : Insect
    {
        public Ant()
        {

        }

        public override IList<Vertex> GetAvailableVertices(Vertex moveFrom, HiveGraph graph)
        {
            var vertices = BasicCheck(moveFrom, graph);

            if (CheckIfSurrounded(moveFrom, graph))
                return new List<Vertex>();

            vertices = vertices.Intersect(graph.GetAdjacentVerticesByCoordList(moveFrom)).ToList();

            vertices = GetAntVerticesByBFS(moveFrom, graph);

            return vertices;
        }

        private IList<Vertex> GetAntVerticesByBFS(Vertex moveFrom, HiveGraph graph) 
        {
            var result = new List<Vertex>();
            var visited = new HashSet<Vertex>();
            var queue = new Queue<Vertex>();

            queue.Enqueue(moveFrom);
            visited.Add(moveFrom);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                if (current.IsEmpty)
                {
                    result.Add(current);
                }

                var adjacent = graph.GetAdjacentVerticesByCoordList(current);

                if (adjacent.Count < 5)
                {
                    foreach (var edge in adjacent)
                    {
                        if (!visited.Contains(edge))
                        {
                            visited.Add(edge);
                            queue.Enqueue(edge);
                        }
                    }
                }
            }

            return result;
        }
    }
}
