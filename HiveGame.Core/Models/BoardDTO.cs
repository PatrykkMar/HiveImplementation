using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveGame.Core.Models
{
    public class BoardDTO
    {
        public PlayerColor playercolor { get; set; }
        public List<VertexDTO> hexes { get; set; }
        public List<long> vertexidtoput { get; set; }
        public bool queenrulemet { get; set; }
    }
}
