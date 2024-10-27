﻿using AutoMapper;
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
    public class HiveBoard
    {
        private Dictionary<(int, int), Vertex> _board;
        private readonly IInsectFactory _factory;
        private readonly IMapper _mapper;
        public HiveBoard(IInsectFactory factory, IMapper mapper)
        {
            _factory = factory;
            _board = new Dictionary<(int x, int y), Vertex>();
            _mapper = mapper;
        }

        public List<Vertex> Vertices
        {
            get
            {
                return _board.Values.ToList();
            }
        }

        public BoardDTO CreateBoardDTO(PlayerColor playerColor)
        {
            //you can put an insect on the empty vertex only if there is no opponent's insect arount
            var toPut = EmptyVertices.Where(x =>
                GetAdjacentVerticesByCoordList(x)
                    .Any(y=>!y.IsEmpty && y.CurrentInsect?.PlayerColor == playerColor)
            &&
                GetAdjacentVerticesByCoordList(x)
                    .Where(y=>!y.IsEmpty)
                    .All(z=>z.CurrentInsect?.PlayerColor == playerColor)
            );

            List<VertexDTO> verticesDTO = new List<VertexDTO>();

            foreach(var vertex in Vertices)
            {
                if(vertex.IsEmpty)
                {
                    var verDTO = new VertexDTO(vertex, playerColor);
                    verticesDTO.Add(verDTO);
                }
                else
                {
                    int zIndex = 0;
                    foreach(var insect in vertex.InsectStack.Reverse())
                    {
                        var insectVertexDTO = new VertexDTO(vertex, playerColor);
                        insectVertexDTO.z = zIndex++;
                        insectVertexDTO.insect = insect.Type;
                        insectVertexDTO.SetVertexToMove(vertex, this, playerColor);
                        verticesDTO.Add(insectVertexDTO);
                    }

                    var lastVertexDTO = new VertexDTO(vertex, playerColor);
                    lastVertexDTO.insect = InsectType.Nothing;
                    lastVertexDTO.z = zIndex;
                    lastVertexDTO.isempty = true;
                    verticesDTO.Add(lastVertexDTO);
                }
            }

            var board = new BoardDTO()
            {
                hexes = verticesDTO,
                vertexidtoput = toPut.Select(x=>x.Id).ToList()
            };

            return board;
        }

        public List<long> GetHexesToMove(Vertex vertex)
        {
            var ids = vertex.CurrentInsect.GetAvailableVertices(vertex, this).Select(x => x.Id).ToList();

            if (ids.Count == 0)
                return null;
            return ids;
        }

        public List<Vertex> NotEmptyVertices
        {
            get
            {
                return _board.Values.Where(x => !x.IsEmpty).ToList();
            }
        }

        public List<Vertex> EmptyVertices
        {
            get
            {
                return _board.Values.Where(x => x.IsEmpty).ToList();
            }
        }

        public bool FirstMoves
        {
            get
            {
                return NotEmptyVertices.Count < 2;
            }
        }

        public bool Move((int, int)? moveFrom, (int, int)? moveTo, Game game)
        {
            if (!moveFrom.HasValue)
                throw new ArgumentException("Empty moveFrom parameter");

            if (!moveTo.HasValue)
                throw new ArgumentException("Empty moveTo parameter");

            var moveFromVertex = GetVertexByCoord(moveFrom.Value);
            var moveToVertex = GetVertexByCoord(moveTo.Value);

            if (moveFromVertex == null || moveFromVertex.IsEmpty)
                throw new ArgumentException("MoveFromVertex not found or empty");

            if (moveToVertex == null)
                throw new ArgumentException("MoveToVertex not found or not empty");

            var moveToVertexEmptyBeforeMove = moveToVertex.IsEmpty;

            if (moveFromVertex.CurrentInsect.PlayerColor != game.GetCurrentPlayer().PlayerColor)
                throw new ArgumentException("Player wants to move opponent's insect");

            var availableHexes = moveFromVertex.CurrentInsect.GetAvailableVertices(moveFromVertex, this);

            if (!availableHexes.Contains(moveToVertex))
                throw new ArgumentException("Insect cannot move there");

            var moveFromInsect = moveFromVertex.InsectStack.Pop();
            moveToVertex.InsectStack.Push(moveFromInsect);
            if(moveFromVertex.IsEmpty)
            {
                RemoveAllEmptyUnconnectedVerticesAround(moveFromVertex);
            }

            if(moveToVertexEmptyBeforeMove)
            {
                AddEmptyVerticesAround(moveToVertex);
            }
            return true;
        }

        public bool Put(InsectType insectType, (int, int)? whereToPut, Game game)
        {
            if(FirstMoves)
            {
                throw new Exception("You have to put first insects first");
            }

            if (insectType == InsectType.Nothing)
                throw new ArgumentNullException("Insect not specified");

            if (!_board.Keys.Any())
                throw new Exception("Cannot put insect using this action on the empty board");

            if (!game.GetCurrentPlayer().RemoveInsectFromPlayerBoard(insectType))
                throw new Exception("Player can't put this insect");

            //adjacency rule

            //check if "where" vertex is empty

            var insect = _factory.CreateInsect(insectType, game.CurrentColorMove);
            var where = GetVertexByCoord(whereToPut);

            if (where == null)
                throw new Exception("Vertex not found");

            where.AddInsectToStack(insect);
            AddEmptyVerticesAround(where);
            return true;
        }

        public void AddVertex(Vertex vertex)
        {
            _board[(vertex.X, vertex.Y)] = vertex;
        }

        public bool PutFirstInsect(InsectType insectType, Game game)
        {
            if (!FirstMoves)
            {
                throw new Exception("There are first insects put");
            }

            if (!game.GetCurrentPlayer().RemoveInsectFromPlayerBoard(insectType))
                throw new Exception("Player can't put this insect");

            Vertex vertex;
            Insect insect = _factory.CreateInsect(insectType, game.CurrentColorMove);

            if (NotEmptyVertices.Count == 0)
                vertex = new Vertex(0, 0, insect);
            else if (NotEmptyVertices.Count == 1)
                vertex = new Vertex(1, 0, insect);
            else
                throw new Exception("It's not first insect");

            AddVertex(vertex);
            AddEmptyVerticesAround(vertex);
            return true;
        }

        private void AddEmptyVerticesAround(Vertex vertex)
        {
            var board = this;
            var adjacentVertices = GetAdjacentVerticesByCoordDict(vertex);
            foreach (var direction in Get2DDirections())
            {

                if (!adjacentVertices.ContainsKey(direction))
                {
                    var emptyVertex = new Vertex(vertex, direction);
                    AddVertex(emptyVertex);
                }
            }
        }

        private void RemoveAllEmptyUnconnectedVerticesAround(Vertex vertex)
        {
            var verticesToDelete = GetAdjacentVerticesByCoordList(vertex).Where(x=>x.IsEmpty && GetAdjacentVerticesByCoordList(x).Where(y=>!y.IsEmpty).Count()==0);

            foreach (var ver in verticesToDelete)
                _board.Remove(ver.Coords);
        }

        public Vertex? GetVertexByCoord(int x, int y)
        {
            if (!_board.ContainsKey((x, y)))
                return null;
            return _board[(x, y)];

        }

        public Vertex? GetVertexByCoord((int X, int Y)? point)
        {
            if(!point.HasValue)
                return null;

            return GetVertexByCoord(point.Value.X, point.Value.Y);
        }


        public Dictionary<Direction, Vertex> GetAdjacentVerticesByCoordDict(Vertex vertex)
        {
            var dict = new Dictionary<Direction, Vertex>();

            foreach (var direction in Get2DDirections())
            {

                var coords = vertex.Coords.Add(NeighborOffsetsDict[direction].To2D());

                var adjacent = GetVertexByCoord(coords);
                if (adjacent != null)
                    dict.Add(direction, adjacent);
            }

            return dict;
        }

        public List<Vertex> GetAdjacentVerticesByCoordList(Vertex vertex)
        {
            return GetAdjacentVerticesByCoordDict(vertex).Values.ToList();
        }

        public Vertex? GetVertexFromVertexAtDirection(Vertex vertex, Direction direction) 
        { 
            var point = vertex.Coords.Add(NeighborOffsetsDict[direction].To2D());
            return GetVertexByCoord(point);
        }
    }
}
