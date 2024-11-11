using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveGame.BusinessLogic.Models.Requests
{
    public class MoveInsectRequest : GameMoveRequest
    {
        public Point2D? MoveFrom { get; set; }
        public Point2D? MoveTo { get; set; }
    }
}
