using QuickGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HiveGame.BusinessLogic.Models.Graph.DirectionConsts;

namespace HiveGame.BusinessLogic.Models.Graph
{
    public class DirectedEdge<TVertex> : Edge<TVertex>
    {
        public Direction Direction { get; private set; }

        public DirectedEdge(TVertex source, TVertex target, Direction directionToTarget)
            : base(source, target)
        {
            Direction = directionToTarget;
        }
    }
}
