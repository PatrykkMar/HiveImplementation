using HiveGame.BusinessLogic.Models.Insects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HiveGame.BusinessLogic.Models.Board.DirectionConsts;

namespace HiveGame.BusinessLogic.Models.Board
{
    public interface IVertex
    {
        long Id { get; set; }
        int X { get; set; }
        int Y { get; set; }
        Point2D Coords { get; set; }
        Insect? CurrentInsect { get; set; }
        bool IsEmpty { get; }
        Stack<Insect> InsectStack { get; }
        string PrintVertex();
        void AddInsectToStack(Insect insect);
    }

    public class Vertex : IVertex
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

        public Vertex(IVertex vertex, Point2D offset) : this(vertex.X + offset.X, vertex.Y + offset.Y)
        {

        }

        public Vertex(IVertex vertex, Direction direction) : this(vertex, NeighborOffsetsDict[direction].To2D())
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

        public Point2D Coords
        {
            get { return new Point2D(X, Y); }
            set
            {
                X = value.X; 
                Y = value.Y;
            }
        }
    }
}
