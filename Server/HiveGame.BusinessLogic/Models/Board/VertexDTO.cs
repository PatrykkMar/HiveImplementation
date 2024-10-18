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
        public long id { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public int z { get; set; }
        public InsectType insect { get; set; }
        public bool highlighted { get; set; }
        public bool isempty { get; set; }
        public PlayerColor? playercolor { get; set; }
        public List<long> vertexidtomove { get; set; } = new List<long>();
        public List<long> vertexidtoput { get; set; } = new List<long>();
    }
}
