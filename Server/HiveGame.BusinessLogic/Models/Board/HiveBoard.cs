using AutoMapper;
using HiveGame.BusinessLogic.Factories;
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
        private Dictionary<(int, int, int), Vertex> _board;
        private readonly IInsectFactory _factory;
        private readonly IMapper _mapper;
        public HiveBoard(IInsectFactory factory, IMapper mapper)
        {
            _factory = factory;
            _board = new Dictionary<(int, int, int), Vertex>();
            _mapper = mapper;
        }

        public List<Vertex> Vertices
        {
            get
            {
                return _board.Values.ToList();
            }
        }

        [Obsolete]
        public List<VertexDTO> VerticesDTO
        {
            get
            {
                var list = Vertices.Select((x,i) => new VertexDTO
                {
                    id = x.Id,
                    x = x.X, 
                    y = x.Y,
                    z = x.Z,
                    insect = x.CurrentInsect != null ? x.CurrentInsect.Type : InsectType.Nothing,
                    highlighted = false,
                    isempty = x.IsEmpty,
                    playercolor = x.CurrentInsect?.PlayerColor
                }).ToList();
                return list;
            }
        }



        public BoardDTO CreateBoardDTO(PlayerColor playerColor)
        {
            //you can put an insect on the empty vertex only if there is no opponent's insect arount
            var toPut = EmptyVertices.Where(x => 
                GetAdjacentVerticesByCoordList(x, ignoreDown: true, ignoreUp: true)
                    .Any(y=>!y.IsEmpty && y.CurrentInsect?.PlayerColor == playerColor)
            &&
                GetAdjacentVerticesByCoordList(x, ignoreDown: true, ignoreUp: true)
                    .Where(y=>!y.IsEmpty)
                    .All(z=>z.CurrentInsect?.PlayerColor == playerColor)
            );

            var list = Vertices.Select((x, i) => new VertexDTO
            {
                id = x.Id,
                x = x.X,
                y = x.Y,
                z = x.Z,
                insect = x.CurrentInsect != null ? x.CurrentInsect.Type : InsectType.Nothing,
                highlighted = false,
                isempty = x.IsEmpty,
                isthisplayerinsect = x.CurrentInsect != null ? x.CurrentInsect.PlayerColor == playerColor : false,
                playercolor = x.CurrentInsect?.PlayerColor,
                vertexidtoput = toPut.Select(x => x.Id).ToList()
            }).ToList();

            var board = new BoardDTO()
            {
                hexes = list,
                vertexidtoput = toPut.Select(x=>x.Id).ToList()
            };

            return board;
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

        public bool Move(Vertex moveFrom, Vertex moveTo, Player player, PlayerColor playerColor)
        {
            if (moveFrom.IsEmpty)
                throw new ArgumentException("There is not insect on the 'moveFrom' vertex");

            if (!moveTo.IsEmpty)
                throw new ArgumentException("'emptyVertex' is not empty");

            if (GetVertexByCoord(moveFrom.X, moveFrom.Y, moveFrom.Z + 1)?.IsEmpty == false)
                throw new ArgumentException("Insect cannot move because there is another insect above him");


            RemoveAllEmptyUnconnectedVerticesAround(moveFrom);

            var insect = moveFrom.CurrentInsect;
            moveTo.CurrentInsect = insect;
            moveFrom.CurrentInsect = null;
            RemoveAllEmptyUnconnectedVerticesAround(moveFrom);

            AddEmptyVerticesAround(moveTo);
            return true;
        }

        public bool Put(InsectType insectType, (int, int, int)? whereToPut, Game game)
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

            where.CurrentInsect = insect;
            AddVertex(where);
            AddEmptyVerticesAround(where);
            return true;
        }

        public void AddVertex(Vertex vertex)
        {
            _board[(vertex.X, vertex.Y, vertex.Z)] = vertex;
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
                vertex = new Vertex(0, 0, 0, insect);
            else if (NotEmptyVertices.Count == 1)
                vertex = new Vertex(1, 0, 0, insect);
            else
                throw new Exception("It's not first insect");

            AddVertex(vertex);
            AddEmptyVerticesAround(vertex);
            return true;
        }

        private void AddEmptyVerticesAround(Vertex vertex, bool ignoreDown = true, bool ignoreUp = false)
        {
            var board = this;
            var adjacentVertices = GetAdjacentVerticesByCoordDict(vertex, ignoreDown, ignoreUp);
            foreach (var direction in (Direction[])Enum.GetValues(typeof(Direction)))
            {
                if (ignoreDown && direction == Direction.Down)
                    continue;

                if (!adjacentVertices.ContainsKey(direction))
                {
                    var emptyVertex = new Vertex(vertex, direction);
                    AddVertex(emptyVertex);
                }
            }
        }

        private bool IsEndgameConditionMet()
        {
            //check if some player's queen is surronded in every side
            throw new NotImplementedException();
        }

        private void RemoveAllEmptyUnconnectedVerticesAround(Vertex vertex)
        {
            var verticesToDelete = GetAdjacentVerticesByCoordList(vertex);

            foreach (var ver in verticesToDelete)
                _board.Remove(ver.Coords);
        }

        public Vertex? GetVertexByCoord(int x, int y, int z)
        {
            if (!_board.ContainsKey((x, y, z)))
                return null;
            return _board[(x, y, z)];

        }

        public Vertex? GetVertexByCoord((int X, int Y, int Z)? point)
        {
            if(!point.HasValue)
                return null;

            return GetVertexByCoord(point.Value.X, point.Value.Y, point.Value.Z);
        }


        public Dictionary<Direction, Vertex> GetAdjacentVerticesByCoordDict(Vertex vertex, bool ignoreDown = true, bool ignoreUp = false)
        {
            var dict = new Dictionary<Direction, Vertex>();

            foreach (var direction in (Direction[])Enum.GetValues(typeof(Direction)))
            {
                if ((ignoreDown && direction == Direction.Down) || (ignoreUp && direction == Direction.Up))
                    continue;

                var offset = NeighborOffsetsDict[direction];

                var adjacent = GetVertexByCoord(vertex.X + offset.dx, vertex.Y + offset.dy, vertex.Z + offset.dz);
                if (adjacent != null)
                    dict.Add(direction, adjacent);
            }

            return dict;
        }

        public List<Vertex> GetAdjacentVerticesByCoordList(Vertex vertex, bool ignoreDown = true, bool ignoreUp = false)
        {
            return GetAdjacentVerticesByCoordDict(vertex, ignoreDown, ignoreUp).Values.ToList();
        }
    }
}
