using AutoMapper;
using HiveGame.BusinessLogic.Factories;
using HiveGame.BusinessLogic.Models.Insects;
using System;
using System.Collections.Generic;
using System.Linq;
using static HiveGame.BusinessLogic.Models.Game.Graph.DirectionConsts;

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

        public List<VertexDTO> VerticesDTO
        {
            get
            {
                return _mapper.Map<List<VertexDTO>>(_board.Values.ToList());
            }
        }

        public List<Vertex> NotEmptyVertices
        {
            get
            {
                return _board.Values.Where(x => !x.IsEmpty).ToList();
            }
        }

        public bool FirstMoves
        {
            get
            {
                return NotEmptyVertices.Count < 2;
            }
        }

        public bool Move(Vertex moveFrom, Vertex moveTo, Player player)
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

        public bool Put(InsectType insectType, (int, int, int)? whereToPut)
        {
            if(FirstMoves)
            {
                throw new Exception("You have to put first insects first");
            }

            if (insectType == InsectType.Nothing)
                throw new ArgumentNullException("Insect not specified");
            if (!_board.Keys.Any())
                throw new Exception("Cannot put insect on the empty board");

            //adjacency rule

            //check if "where" vertex is empty

            var insect = _factory.CreateInsect(insectType);
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

        public bool PutFirstInsect(InsectType insectType)
        {
            if (!FirstMoves)
            {
                throw new Exception("There are first insects put");
            }

            Vertex vertex;
            Insect insect = _factory.CreateInsect(insectType);

            if (_board.Count == 0)
                vertex = new Vertex(0, 0, 0, insect);
            else if (_board.Count == 1)
                vertex = new Vertex(1, 0, 0, insect);
            else
                throw new Exception("It's not first insect");

            AddVertex(vertex);
            AddEmptyVerticesAround(vertex);
            return true;
        }

        private void AddEmptyVerticesAround(Vertex vertex, bool ignoreDown = true)
        {
            var board = this;
            var adjacentVertices = GetAdjacentVerticesByCoordDict(vertex, ignoreDown);
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


        public Dictionary<Direction, Vertex> GetAdjacentVerticesByCoordDict(Vertex vertex, bool ignoreDown = true)
        {
            var dict = new Dictionary<Direction, Vertex>();

            foreach (var direction in (Direction[])Enum.GetValues(typeof(Direction)))
            {
                if (ignoreDown && direction == Direction.Down)
                    continue;

                var offset = NeighborOffsetsDict[direction];

                var adjacent = GetVertexByCoord(vertex.X + offset.dx, vertex.Y + offset.dy, vertex.Z + offset.dz);
                if (adjacent != null)
                    dict.Add(direction, adjacent);
            }

            return dict;
        }

        public List<Vertex> GetAdjacentVerticesByCoordList(Vertex vertex)
        {
            return GetAdjacentVerticesByCoordDict(vertex).Values.ToList();
        }
    }
}
