using HiveGame.BusinessLogic.Models.Insects;
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
        public (int, int, int)? WhereToPut { get; set; }
        public Player? Player { get; set; }
    }
}
