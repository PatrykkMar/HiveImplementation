using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveGame.BusinessLogic.Models.Extensions
{
    [Obsolete]
    public static class PointExtensions
    {
        public static (int, int, int) Add(this (int, int, int) tuple1, (int, int, int) tuple2)
        {
            return (tuple1.Item1 + tuple2.Item1, tuple1.Item2 + tuple2.Item2, tuple1.Item3 + tuple2.Item3);
        }

        public static (int, int) Add(this (int, int) tuple1, (int, int) tuple2)
        {
            return (tuple1.Item1 + tuple2.Item1, tuple1.Item2 + tuple2.Item2);
        }

        public static (int, int) To2D(this (int, int, int) tuple1)
        {
            return (tuple1.Item1, tuple1.Item2);
        }
    }
}
