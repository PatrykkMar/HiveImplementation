using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveGame.BusinessLogic.Models.Requests
{
    public class MoveRequest
    {
        public (int, int)? MoveFrom { get; set; }
        public (int, int)? MoveTo { get; set; }
        public (int, int)? Player { get; set; }
    }
}
