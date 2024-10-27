using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HiveGame.BusinessLogic.Models.Graph
{
    public class DirectionConsts
    {
        public enum Direction
        {
            TopRight, Right, BottomRight, BottomLeft, Left, TopLeft, Up, Down
        }

        public static readonly Dictionary<Direction, (int dx, int dy, int dz)> NeighborOffsetsDict = new()
        {
            { Direction.Right, (1, 0, 0) },
            { Direction.TopRight, (1, -1, 0) },
            { Direction.TopLeft, (0, -1, 0) },
            { Direction.Left, (-1, 0, 0) },
            { Direction.BottomLeft, (-1, 1, 0) },
            { Direction.BottomRight, (0, 1, 0) },
            { Direction.Up, (0, 0, 1) },
            { Direction.Down, (0, 0, -1) }
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
                Direction.Up => Direction.Down,
                Direction.Down => Direction.Up,
                _ => throw new ArgumentException("Invalid direction")
            };
        }

        public static Direction[] Get2DDirections()
        {
            return Enum.GetValues<Direction>()
                .Where(x=> x != Direction.Up && x != Direction.Down)
                .ToArray();
        }

        public static Direction NextDirection(Direction direction, bool includeUpAndDown = false)
        {
            int amountOfDirection = includeUpAndDown ? 8 : 6;

            return (Direction)((int)(direction + 1) % amountOfDirection);
        }
    }
}
