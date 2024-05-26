using QuickGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HiveGame.BusinessLogic.Models.Graph.DirectionConsts;

namespace HiveGame.BusinessLogic.Models.Graph
{
    public class DirectionConsts
    {
        public enum Direction
        {
            TopRight, Right, BottomRight, BottomLeft, Left, TopLeft
        }

        public static readonly Dictionary<Direction, (int dx, int dy)> NeighborOffsetsDict = new()
        {
            { Direction.Right, (1, 0) },
            { Direction.TopRight, (1, -1) },
            { Direction.TopLeft, (0, -1) },
            { Direction.Left, (-1, 0) },
            { Direction.BottomLeft, (-1, 1) },
            { Direction.BottomRight, (0, 1) }
        };

        public static readonly (int dx, int dy)[] NeighborOffsets = new (int, int)[]
        {
            (1, 0),
            (1, -1),
            (0, -1), 
            (-1, 0),
            (-1, 1),
            (0, 1)
        };

        public static Direction OppositeDirection(Direction direction)
        {
            return direction switch
            {
                Direction.Right => Direction.Left,
                Direction.TopRight => Direction.BottomLeft,
                Direction.TopLeft => Direction.BottomRight,
                Direction.Left => Direction.Right,
                Direction.BottomLeft => Direction.TopRight,
                Direction.BottomRight => Direction.TopLeft,
                _ => throw new ArgumentException("Invalid direction")
            };
        }
    }
}
