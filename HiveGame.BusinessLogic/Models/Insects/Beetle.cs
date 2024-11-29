using HiveGame.BusinessLogic.Models.Extensions;
using HiveGame.BusinessLogic.Models.Board;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HiveGame.Core.Models;

namespace HiveGame.BusinessLogic.Models.Insects
{
    public class Beetle : Insect
    {
        public Beetle(PlayerColor color)
        {
            Type = InsectType.Beetle;
            PlayerColor = color;
        }

        public override InsectValidationResult GetAvailableVertices(IVertex moveFrom, IHiveBoard board)
        {
            var result = new InsectValidationResult();

            List<IVertex> vertices = BasicCheck(moveFrom, board, out string? whyMoveImpossible, onlyEmpty: false);

            if (vertices.Count == 0)
            {
                result.ReasonWhyEmpty = whyMoveImpossible;
                return result;
            }

            var freeHexesAround = CheckNotSurroundedFields(moveFrom, board)
                .Union(board.GetAdjacentVerticesByCoordList(moveFrom).Where
                    (
                        x => !x.IsEmpty || //moving on insect
                        (x.IsEmpty && moveFrom.InsectStack.Count > 1) //going down
                    )
                ).ToList(); //beetle can move on insect

            if (freeHexesAround.Count == 0)
            {
                result.ReasonWhyEmpty = "This insect is surrounded and can't move";
                return result;
            }

            result.AvailableVertices = vertices.Intersect(freeHexesAround).ToList();

            return result;
        }
    }
}
