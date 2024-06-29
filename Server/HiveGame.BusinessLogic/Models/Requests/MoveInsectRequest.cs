using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveGame.BusinessLogic.Models.Requests
{
    public class MoveInsectRequest : GameMoveRequest
    {
        public (int, int, int)? MoveFrom { get; set; }
        public (int, int, int)? MoveTo { get; set; }
    }
}
