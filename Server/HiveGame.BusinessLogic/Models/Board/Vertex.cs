using HiveGame.BusinessLogic.Models.Extensions;
using HiveGame.BusinessLogic.Models.Insects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HiveGame.BusinessLogic.Models.Graph.DirectionConsts;

namespace HiveGame.BusinessLogic.Models.Graph
{
    public class Vertex
    {
        public static long MaxId { get; set; } = 0;
        public Vertex() { }

        public Vertex(int x, int y)
        {
            Id = MaxId++;
            X = x;
            Y = y;
        }

        public Vertex(int x, int y, Insect insect) : this(x, y)
        {
            CurrentInsect = insect;
        }

        public Vertex(Vertex vertex, (int dx, int dy) offset) : this(vertex.X + offset.dx, vertex.Y + offset.dy)
        {

        }

        public Vertex(Vertex vertex, Direction direction) : this(vertex, NeighborOffsetsDict[direction].To2D())
        {

        }

        public Insect? CurrentInsect
        {
            get
            {
                if (InsectStack.TryPeek(out Insect? result))
                    return result;

                return null;
            }
            set
            {
                InsectStack.Clear();
                InsectStack.Push(value);
            }
        }

        public void AddInsectToStack(Insect insect)
        {
            InsectStack.Push(insect);
        }

        public Stack<Insect> InsectStack { get; set; } = new Stack<Insect>();

        public bool IsEmpty
        {
            get
            {
                return CurrentInsect == null;
            }
        }

        public string PrintVertex()
        {
            return $"({X},{Y}) {(IsEmpty ? "empty " : " ")}{(CurrentInsect != null ? "hasInsect" : "")}";
        }
        public long Id { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public (int x, int y, int z) Coords3D
        {
            get { return (X,Y,0); }
        }

        public (int x, int y) Coords
        {
            get { return (X, Y); }
        }
    }
}
