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

        public Vertex() { }

        public Vertex(int x, int y, int z) : this(x, y)
        {
            Z = z;
        }

        public Vertex(int x, int y)
        {
            X = x;
            Y = y;
            Z = 0;
        }

        public Vertex(int x, int y, int z, Insect insect)
        {
            X = x;
            Y = y;
            Z = z;
            CurrentInsect = insect;
        }

        public Vertex(Vertex vertex, (int dx, int dy, int dz) offset)
        {
            X = vertex.X + offset.dx;
            Y = vertex.Y + offset.dy;
            Z = vertex.Z + offset.dz;
        }

        public Vertex(Vertex vertex, Direction direction)
        {
            var offset = NeighborOffsetsDict[direction];
            X = vertex.X + offset.dx;
            Y = vertex.Y + offset.dy;
            Z = vertex.Z + offset.dz;
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

        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public (int, int, int) Coords
        {
            get { return (X,Y,Z); }
        }
    }
}
