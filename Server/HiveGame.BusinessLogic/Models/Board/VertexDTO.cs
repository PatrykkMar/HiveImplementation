using HiveGame.BusinessLogic.Models.Insects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveGame.BusinessLogic.Models.Graph
{
    public class VertexDTO
    {
        public int x { get; set; }
        public int y { get; set; }
        public int z { get; set; }
        public InsectType insect { get; set; }
        public bool highlighted { get; set; }
    }
}
