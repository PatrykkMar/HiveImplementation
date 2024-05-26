using QuickGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveGame.BusinessLogic.Models.Graph
{
    public class Insect
    {
        public Insect(InsectType type)
        {
            Type = type;
        }

        public InsectType Type { get; set; }

        public long X { get; set; }
        public long Y { get; set; }
    }

    public enum InsectType
    {
        TypeOne, TypeTwo, TypeThree, TypeFour
    }
}
