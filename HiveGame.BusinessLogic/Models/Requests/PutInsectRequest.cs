using HiveGame.BusinessLogic.Models.Insects;
using HiveGame.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveGame.BusinessLogic.Models.Requests
{
    public class PutInsectRequest : GameMoveRequest
    {
        public InsectType InsectToPut { get; set; }
        public Point2D? WhereToPut { get; set; }
    }
}
