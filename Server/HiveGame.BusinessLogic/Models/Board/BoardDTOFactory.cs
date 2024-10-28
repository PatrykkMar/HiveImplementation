using AutoMapper;
using HiveGame.BusinessLogic.Factories;
using HiveGame.BusinessLogic.Models.Extensions;
using HiveGame.BusinessLogic.Models.Insects;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using static HiveGame.BusinessLogic.Models.Graph.DirectionConsts;

namespace HiveGame.BusinessLogic.Models.Graph
{
    public class BoardDTOFactory
    {
        public static BoardDTO CreateBoardDTO(HiveBoard board, PlayerColor playerColor)
        {
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
                    var verDTO = new VertexDTO(vertex, playerColor);
                    verticesDTO.Add(verDTO);
                }
                else
                {
                    int zIndex = 0;
                    foreach (var insect in vertex.InsectStack.Reverse())
                    {
                        var insectVertexDTO = new VertexDTO(vertex, playerColor);
                        insectVertexDTO.z = zIndex++;
                        insectVertexDTO.insect = insect.Type;
                        insectVertexDTO.SetVertexToMove(vertex, board, playerColor);
                        verticesDTO.Add(insectVertexDTO);
                    }

                    var lastVertexDTO = new VertexDTO(vertex, playerColor);
                    lastVertexDTO.insect = InsectType.Nothing;
                    lastVertexDTO.z = zIndex;
                    lastVertexDTO.isempty = true;
                    verticesDTO.Add(lastVertexDTO);
                }
            }

            var boardDto = new BoardDTO()
            {
                hexes = verticesDTO,
                vertexidtoput = toPut.Select(x => x.Id).ToList()
            };

            return boardDto;
        }
    }
}
