using HiveGame.BusinessLogic.Models.Graph;
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
        public long X { get; set; }
        public long Y { get; set; }
        public InsectType? Insect { get; set; }
        public VertexDTO(Vertex vertex) 
        { 
            X = vertex.X;
            Y = vertex.Y;
            Insect = vertex.CurrentInsect?.Type;
        }
    }
}
