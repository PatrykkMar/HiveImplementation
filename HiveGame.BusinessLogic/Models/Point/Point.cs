using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HiveGame.BusinessLogic.Models
{
    public class Point2D
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point2D(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Point2D Add(Point2D other)
        {
            return new Point2D(X + other.X, Y + other.Y);
        }

        public Point2D Copy()
        {
            return new Point2D(X, Y);
        }

        public override bool Equals(object obj)
        {
            if (obj is Point2D other)
            {
                return X == other.X && Y == other.Y;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        public override string ToString()
        {
            return $"{X}:{Y}";
        }

        public static string ToString(int X, int Y)
        {
            return $"{X}:{Y}";
        }
    }

    public class Point3D : Point2D
    {
        public int Z { get; set; }

        public Point3D(int x, int y, int z) : base(x, y)
        {
            Z = z;
        }

        public Point3D Add(Point3D other)
        {
            return new Point3D(X + other.X, Y + other.Y, Z + other.Z);
        }

        public new Point3D Copy()
        {
            return new Point3D(X, Y, Z);
        }

        public override bool Equals(object obj)
        {
            if (obj is Point3D other)
            {
                return X == other.X && Y == other.Y && Z == other.Z;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z);
        }

        public Point2D To2D()
        {
            return new Point2D(X, Y);
        }
    }
}