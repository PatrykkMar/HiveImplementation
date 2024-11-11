using HiveGame.BusinessLogic.Models.Insects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveGame.BusinessLogic.Models.Requests
{
    public class PutInsectRequest
    {
        public InsectType InsectToPut { get; set; }
        public Point2D? WhereToPut { get; set; }
        public string PlayerId { get; set; }
    }
}
