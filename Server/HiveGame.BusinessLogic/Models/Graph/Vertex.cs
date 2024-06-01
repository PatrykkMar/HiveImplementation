using HiveGame.BusinessLogic.Models.Insects;
using QuickGraph;
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

        public Vertex() { }

        public Vertex(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Vertex(int x, int y, Insect insect)
        {
            X = x;
            Y = y;
            CurrentInsect = insect;
        }

        public Vertex(Vertex vertex, (int dx, int dy) offset)
        {
            X = vertex.X + offset.dx;
            Y = vertex.Y + offset.dy;
        }

        public Vertex(Vertex vertex, Direction direction)
        {
            var offset = NeighborOffsetsDict[direction];
            X = vertex.X + offset.dx;
            Y = vertex.Y + offset.dy;
        }

        public Insect? CurrentInsect { get; set; }

        public bool IsEmpty
        {
            get
            {
                return CurrentInsect == null;
            }
        }

        public long X { get; set; }
        public long Y { get; set; }

        public Dictionary<Direction, Vertex> GetAdjacentVerticesDict(IList<Vertex> vertices)
        {
            var dict = new Dictionary<Direction, Vertex>();
            var leftUp = vertices.FirstOrDefault(x => x.X == X && x.Y == Y + 1);
            if (leftUp != null) dict.Add(Direction.TopLeft, leftUp);

            var rightUp = vertices.FirstOrDefault(x => x.X == X + 1 && x.Y == Y + 1);
            if (rightUp != null) dict.Add(Direction.TopRight, rightUp);

            var left = vertices.FirstOrDefault(x => x.X == X - 1 && x.Y == Y);
            if (left != null) dict.Add(Direction.Left, left);

            var right = vertices.FirstOrDefault(x => x.X == X + 1 && x.Y == Y);
            if (right != null) dict.Add(Direction.Right, right);

            var leftDown = vertices.FirstOrDefault(x => x.X == X && x.Y == Y - 1);
            if (leftDown != null) dict.Add(Direction.BottomLeft, leftDown);

            var rightDown = vertices.FirstOrDefault(x => x.X == X - 1 && x.Y == Y - 1);
            if (rightDown != null) dict.Add(Direction.BottomRight, rightDown);

            return dict;
        }

        public List<Vertex> GetAdjacentVerticesList(IList<Vertex> vertices)
        {
            return GetAdjacentVerticesDict(vertices).Values.ToList();
        }
    }
}
