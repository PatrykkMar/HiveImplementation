using AutoMapper;
using HiveGame.BusinessLogic.Factories;
using HiveGame.BusinessLogic.Models.Extensions;
using HiveGame.BusinessLogic.Models.Insects;
using HiveGame.DataAccess.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using static HiveGame.BusinessLogic.Models.Board.DirectionConsts;

namespace HiveGame.BusinessLogic.Models.Board
{
    public interface  IHiveBoard
    {
        List<IVertex> Vertices { get; }
        List<Insect> AllInsects { get; }
        List<IVertex> NotEmptyVertices { get; }
        List<IVertex> EmptyVertices { get; }
        bool FirstMoves { get; }

        void AddVertex(IVertex vertex);
        void AddEmptyVerticesAround(IVertex vertex);
        void RemoveAllEmptyUnconnectedVerticesAround(IVertex vertex);
        IVertex? GetVertexByCoord(int x, int y);
        IVertex? GetVertexByCoord(Point2D? point);
        Dictionary<Direction, IVertex> GetAdjacentVerticesByCoordDict(IVertex vertex);
        List<IVertex> GetAdjacentVerticesByCoordList(IVertex vertex);
        IVertex? GetVertexFromVertexAtDirection(IVertex vertex, Direction direction);
        List<long> GetHexesToMove(IVertex vertex, out string? whyMoveImpossible);
    }

    public class HiveBoard :  IHiveBoard
    {
        private Dictionary<string, IVertex> _board;
        public HiveBoard()
        {
            _board = new Dictionary<string, IVertex>();
        }

        public List<IVertex> Vertices
        {
            get
            {
                return _board.Values.ToList();
            }
        }

        public List<Insect> AllInsects
        {
            get
            {
                return Vertices.SelectMany(x=>x.InsectStack).ToList();
            }
        }

        public List<long> GetHexesToMove(IVertex vertex, out string? whyMoveImpossible)
        {
            var availableVerticesResult = vertex.CurrentInsect.GetAvailableVertices(vertex, this);
            whyMoveImpossible = "";
            whyMoveImpossible = availableVerticesResult.ReasonWhyEmpty;

            var ids = availableVerticesResult
                .AvailableVertices
                .Select(x => x.Id)
                .ToList();

            if (ids.Count == 0)
                return null;
            return ids;
        }

        public List<IVertex> NotEmptyVertices
        {
            get
            {
                return _board.Values.Where(x => !x.IsEmpty).ToList();
            }
        }

        public List<IVertex> EmptyVertices
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

        public void AddVertex(IVertex vertex)
        {
            _board[vertex.Coords.ToString()] = vertex;
        }

        public void AddEmptyVerticesAround(IVertex vertex)
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

        public void RemoveAllEmptyUnconnectedVerticesAround(IVertex vertex)
        {
            var verticesToDelete = GetAdjacentVerticesByCoordList(vertex).Where(x=>x.IsEmpty && GetAdjacentVerticesByCoordList(x).Where(y=>!y.IsEmpty).Count()==0);

            foreach (var ver in verticesToDelete)
                _board.Remove(ver.Coords.ToString());
        }

        public IVertex? GetVertexByCoord(int x, int y)
        {
            if (!_board.ContainsKey(Point2D.ToString(x, y)))
                return null;
            return _board[Point2D.ToString(x, y)];

        }

        public IVertex? GetVertexByCoord(Point2D? point)
        {
            if(point == null)
                return null;

            return GetVertexByCoord(point.X, point.Y);
        }


        public Dictionary<Direction, IVertex> GetAdjacentVerticesByCoordDict(IVertex vertex)
        {
            var dict = new Dictionary<Direction, IVertex>();

            foreach (var direction in Get2DDirections())
            {

                var coords = vertex.Coords.Add(NeighborOffsetsDict[direction].To2D());

                var adjacent = GetVertexByCoord(coords);
                if (adjacent != null)
                    dict.Add(direction, adjacent);
            }

            return dict;
        }

        public List<IVertex> GetAdjacentVerticesByCoordList(IVertex vertex)
        {
            return GetAdjacentVerticesByCoordDict(vertex).Values.ToList();
        }

        public IVertex? GetVertexFromVertexAtDirection(IVertex vertex, Direction direction) 
        { 
            var point = vertex.Coords.Add(NeighborOffsetsDict[direction].To2D());
            return GetVertexByCoord(point);
        }
    }
}
