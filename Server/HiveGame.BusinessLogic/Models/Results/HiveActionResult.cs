using HiveGame.BusinessLogic.Models.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveGame.BusinessLogic.Models.Results
{
    public class HiveActionResult
    {
        public List<VertexDTO> CurrentBoard { get; set; }
        public string Message { get; set; }
        public bool IsOperationSuccess { get; set; } 
    }
}
