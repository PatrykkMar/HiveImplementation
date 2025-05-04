using HiveGame.BusinessLogic.Models.Insects;
using HiveGame.Core.Models;

namespace HiveGame.BusinessLogic.Models.Board
{
    public class BoardDTOFactory
    {
        public static BoardDTO CreateBoardDTO(IHiveBoard board, PlayerColor playerColor, int turn)
        {
            var boardDto = new BoardDTO();

            var toPut = board.EmptyVertices.Where(x =>
                board.GetAdjacentVerticesByCoordList(x)
                    .Any(y => !y.IsEmpty && y.CurrentInsect?.PlayerColor == playerColor)
            &&
                board.GetAdjacentVerticesByCoordList(x)
                    .Where(y => !y.IsEmpty)
                    .All(z => z.CurrentInsect?.PlayerColor == playerColor)
            );

            List<VertexDTO> verticesDTO = new List<VertexDTO>();

            foreach (var vertex in board.Vertices)
            {
                if (vertex.IsEmpty)
                {
                    var verDTO = CreateVertexDTO(vertex, playerColor);
                    verticesDTO.Add(verDTO);
                }
                else
                {
                    int zIndex = 0;
                    foreach (var insect in vertex.InsectStack.Reverse())
                    {
                        var insectVertexDTO = CreateVertexDTO(vertex, playerColor);
                        insectVertexDTO.z = zIndex++;
                        insectVertexDTO.insect = insect.Type;
                        insectVertexDTO.playercolor = insect.PlayerColor;
                        SetVertexToMove(insectVertexDTO, vertex, board, playerColor);
                        verticesDTO.Add(insectVertexDTO);
                    }

                    var lastVertexDTO = CreateVertexDTO(vertex, playerColor);
                    lastVertexDTO.insect = InsectType.Nothing;
                    lastVertexDTO.z = zIndex;
                    lastVertexDTO.isempty = true;
                    verticesDTO.Add(lastVertexDTO);
                }
            }
            boardDto.playercolor = playerColor;
            boardDto.hexes = verticesDTO;
            boardDto.vertexidtoput = toPut.Select(x => x.Id).ToList();
            boardDto.queenrulemet = turn != 4 || board.AllInsects.Where(x => x.Type == InsectType.Queen).Any(x => x.PlayerColor == playerColor);

            return boardDto;
        }

        public static VertexDTO CreateVertexDTO(IVertex vertex, PlayerColor playerColor)
        {
            var vertexDTO = new VertexDTO();
            vertexDTO.id = vertex.Id;
            vertexDTO.x = vertex.X;
            vertexDTO.y = vertex.Y;
            vertexDTO.z = 0;
            vertexDTO.insect = vertex.CurrentInsect != null ? vertex.CurrentInsect.Type : InsectType.Nothing;
            vertexDTO.highlighted = false;
            vertexDTO.isempty = vertex.IsEmpty;
            vertexDTO.isthisplayerinsect = vertex.CurrentInsect != null ? vertex.CurrentInsect.PlayerColor == playerColor : false;
            vertexDTO.playercolor = vertex.CurrentInsect?.PlayerColor ?? PlayerColor.NoColor;
            return vertexDTO;
        }

        public static void SetVertexToMove(VertexDTO dto, IVertex vertex, IHiveBoard board, PlayerColor playerColor)
        {
            var hexesToMove = board.GetHexesToMove(vertex, out string? whyMoveImpossible);
            dto.vertexidtomove = vertex.CurrentInsect != null && vertex.CurrentInsect.PlayerColor == playerColor ? hexesToMove : null;

            if (!string.IsNullOrEmpty(whyMoveImpossible))
                dto.reasonwhymoveimpossible = whyMoveImpossible;
        }

    }
}
