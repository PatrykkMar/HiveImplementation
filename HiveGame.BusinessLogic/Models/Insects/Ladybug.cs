using HiveGame.BusinessLogic.Models.Board;
using HiveGame.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HiveGame.BusinessLogic.Models.Board.DirectionConsts;

namespace HiveGame.BusinessLogic.Models.Insects
{
    public class Ladybug : Insect
    {
        public Ladybug(PlayerColor color)
        {
            Type = InsectType.Ladybug;
            PlayerColor = color;
        }

        public override InsectValidationResult GetAvailableVertices(IVertex moveFrom, IHiveBoard board)
        {
            var result = new InsectValidationResult();

            var vertices = BasicCheck(moveFrom, board, out string? whyMoveImpossible);

            if (vertices.Count == 0)
            {
                result.ReasonWhyEmpty = whyMoveImpossible;
                return result;
            }

            var possibleMoves = GetPossibleMovesForLadybug(moveFrom, board);

            result.AvailableVertices = vertices
                .Where(x => possibleMoves.Any(y => y.Equals(x)))
                .Distinct()
                .ToList();

            return result;
        }

        public List<IVertex> GetPossibleMovesForLadybug(IVertex moveFrom, IHiveBoard board)
        {
            var surroundingNotEmptyVertices = board.GetAdjacentVerticesByCoordList(moveFrom).Where(x => !x.IsEmpty).ToList();

            var possibleMoves = board
                .GetAdjacentVerticesByCoordList(moveFrom)
                .Where(x => !x.IsEmpty)
                .SelectMany(y => board.GetAdjacentVerticesByCoordList(y).Where(a => a != moveFrom))
                .Where(x => !x.IsEmpty)
                .Distinct()
                .SelectMany(z => board.GetAdjacentVerticesByCoordList(z))
                .Where(x => x.IsEmpty)
                .Distinct()
                .ToList();

            return possibleMoves;
        }
    }
}
