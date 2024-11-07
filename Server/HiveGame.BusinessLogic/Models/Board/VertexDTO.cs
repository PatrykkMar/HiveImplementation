using HiveGame.BusinessLogic.Models.Insects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveGame.BusinessLogic.Models.Board
{
    public class VertexDTO
    {
        public VertexDTO(Vertex vertex, PlayerColor playerColor)
        {
            id = vertex.Id;
            x = vertex.X;
            y = vertex.Y;
            z = 0;
            insect = vertex.CurrentInsect != null ? vertex.CurrentInsect.Type : InsectType.Nothing;
            highlighted = false;
            isempty = vertex.IsEmpty;
            isthisplayerinsect = vertex.CurrentInsect != null ? vertex.CurrentInsect.PlayerColor == playerColor : false;
            playercolor = vertex.CurrentInsect?.PlayerColor;
        }

        public void SetVertexToMove(Vertex vertex, HiveBoard board, PlayerColor playerColor)
        {
            var hexesToMove = board.GetHexesToMove(vertex, out string? whyMoveImpossible);
            vertexidtomove = vertex.CurrentInsect != null && vertex.CurrentInsect.PlayerColor == playerColor ? hexesToMove : null;

            if(!string.IsNullOrEmpty(whyMoveImpossible))
                reasonwhymoveimpossible = whyMoveImpossible;
        }

        public long id { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public int z { get; set; }
        public InsectType insect { get; set; }
        public bool highlighted { get; set; }
        public bool isempty { get; set; }
        public bool isthisplayerinsect { get; set; }
        public PlayerColor? playercolor { get; set; }
        public List<long> vertexidtomove { get; set; } = new List<long>();
        public string reasonwhymoveimpossible { get; set; }
    }
}
