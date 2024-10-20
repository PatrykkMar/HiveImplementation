using HiveGame.BusinessLogic.Models.Graph;
using HiveGame.BusinessLogic.Models.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HiveGame.BusinessLogic.Models.Graph.DirectionConsts;

namespace HiveGame.BusinessLogic.Models.Insects
{
    public abstract class Insect
    {
        public Insect()
        {
        }

        public Insect(InsectType type, PlayerColor color)
        {
            Type = type;
            PlayerColor = color;
        }

        public InsectType Type { get; set; }
        public PlayerColor PlayerColor { get; set; }

        public IList<Vertex> BasicCheck(Vertex moveFrom, HiveBoard board)
        {
            var vertices = board.Vertices.ToList();

            var verticesToRemove = board.GetAdjacentVerticesByCoordList(moveFrom)
                .Where(x => board.GetAdjacentVerticesByCoordList(x).Count == 1); //cannot move on empty vertex which would be deleted after move

            foreach(var v in verticesToRemove)
                vertices.Remove(v);

            vertices.Remove(moveFrom); //cannot stay during move

            return vertices;
        }

        //insect is surrounded (and most of them can't move) if there are 2 parallel pairs around it
        public bool CheckIfSurrounded(Vertex moveFrom, HiveBoard board)
        {
            if(board.GetAdjacentVerticesByCoordList(moveFrom).Count >= 5)
                return true;

            var directions = board
                .GetAdjacentVerticesByCoordList(moveFrom)
                .Where(x => !x.IsEmpty)
                .ToList();


            Direction[]? pair = null;

            var next = (Direction d) => { return (Direction)((int)(d + 1) % 6); };

            //TODO: New surrounding condition
            //for (Direction i = 0; (int)i <= 5; i++)
            //{
            //    if(directions.Contains(i) && directions.Contains(next(i)))
            //    {
            //        pair = new Direction[2] {i, next(i) };
            //        break;
            //    }
            //}

            //if(pair != null)
            //{
            //    if (directions.Contains(OppositeDirection(pair[0])) && directions.Contains(OppositeDirection(pair[1])))
            //    {
            //        return true;
            //    }
            //}

            return false;
        }

        public abstract IList<Vertex> GetAvailableVertices(Vertex moveFrom, HiveBoard board);
    }

    public enum InsectType
    {
        Nothing=-1, Queen=0, Ant=1, Spider=2, Grasshopperm=3, Beetle=4
    }
}
