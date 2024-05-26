using HiveGame.BusinessLogic.Models.Graph;
using HiveGame.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveGame.BusinessLogic.Models.Requests
{
    public class PutRequest
    {
        public InsectType InsectToPut { get; set; }
        public Point? WhereToPut { get; set; }
        public Player? Player { get; set; }
    }
}
