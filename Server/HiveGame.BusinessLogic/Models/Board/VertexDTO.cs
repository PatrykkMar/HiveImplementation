using HiveGame.BusinessLogic.Models.Insects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveGame.BusinessLogic.Models.Graph
{
    [Serializable]
    public class VertexDTO
    {
        public long x { get; set; }
        public long y { get; set; }
        public long z { get; set; }
        public int insect { get; set; }
        public bool highlighted { get; set; }
        public VertexDTO(Vertex vertex)
        {
            x = vertex.X;
            y = vertex.Y;
            z = vertex.Z;
            insect = (int?)vertex.CurrentInsect?.Type ?? -1;
        }

        public VertexDTO(Vertex vertex, IEnumerable<Vertex> highlightedVertices) : this(vertex)
        {
            highlighted = highlightedVertices.Any(x => x.X == vertex.X && x.Y == vertex.Y && x.Z == vertex.Z);
        }
    }
}
