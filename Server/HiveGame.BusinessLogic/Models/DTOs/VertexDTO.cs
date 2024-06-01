using HiveGame.BusinessLogic.Models.Graph;
using HiveGame.BusinessLogic.Models.Insects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveGame.BusinessLogic.Models.DTOs
{
    [Serializable]
    public class VertexDTO
    {
        public long x { get; set; }
        public long y { get; set; }
        public InsectType? insect { get; set; }
        public VertexDTO(Vertex vertex) 
        { 
            x = vertex.X;
            y = vertex.Y;
            insect = vertex.CurrentInsect?.Type;
        }
    }
}
