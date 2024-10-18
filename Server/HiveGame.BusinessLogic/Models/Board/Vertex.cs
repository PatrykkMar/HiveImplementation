using HiveGame.BusinessLogic.Models.Insects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HiveGame.BusinessLogic.Models.Game.Graph.DirectionConsts;

namespace HiveGame.BusinessLogic.Models.Graph
{
    public class Vertex
    {
        public static long MaxId { get; set; } = 0;
        public Vertex() { }

        public Vertex(int x, int y, int z)
        {
            Id = MaxId++;
            X = x;
            Y = y;
            Z = z;
        }

        public Vertex(int x, int y) : this(x,y,0)
        {

        }

        public Vertex(int x, int y, int z, Insect insect) : this(x, y, z)
        {
            CurrentInsect = insect;
        }

        public Vertex(Vertex vertex, (int dx, int dy, int dz) offset) : this(vertex.X + offset.dx, vertex.Y + offset.dy, vertex.Z + offset.dz)
        {

        }

        public Vertex(Vertex vertex, Direction direction) : this(vertex, NeighborOffsetsDict[direction])
        {

        }

        public Insect? CurrentInsect { get; set; }

        public bool IsEmpty
        {
            get
            {
                return CurrentInsect == null;
            }
        }

        public string PrintVertex()
        {
            return $"({X},{Y},{Z}) {(IsEmpty ? "empty " : " ")}{(CurrentInsect != null ? "hasInsect" : "")}";
        }
        public long Id { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public (int, int, int) Coords
        {
            get { return (X,Y,Z); }
        }
    }
}
