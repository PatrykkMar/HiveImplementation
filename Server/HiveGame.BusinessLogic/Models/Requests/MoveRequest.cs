using HiveGame.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveGame.BusinessLogic.Models.Requests
{
    public class MoveRequest
    {
        public Point? MoveFrom { get; set; }
        public Point? MoveTo { get; set; }
        public Point? Player { get; set; }
    }
}
