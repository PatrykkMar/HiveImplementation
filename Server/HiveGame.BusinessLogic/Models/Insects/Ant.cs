using HiveGame.BusinessLogic.Models.Board;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveGame.BusinessLogic.Models.Insects
{
    public class Ant : Insect
    {
        public Ant(PlayerColor color)
        {
            Type = InsectType.Ant;
            PlayerColor = color;
        }

        public override InsectValidationResult GetAvailableVertices(Vertex moveFrom, HiveBoard board)
        {
            var result = new InsectValidationResult();

            List<Vertex> vertices = BasicCheck(moveFrom, board, out string? whyMoveImpossible);

            if(vertices.Count == 0) 
            {
                result.ReasonWhyEmpty = whyMoveImpossible;
                return result;
            }

            var freeHexesAround = CheckNotSurroundedFields(moveFrom, board);

            if (freeHexesAround.Count == 0)
            {
                result.ReasonWhyEmpty = "This insect is surrounded and can't move";
                return result;
            }

            List<Vertex> hexesToMoveFromfreeHexes = freeHexesAround
                .SelectMany(x => GetVerticesByBFS(moveFrom, board))
                .Distinct()
                .ToList();

            result.AvailableVertices = vertices.Intersect(hexesToMoveFromfreeHexes).ToList();

            return result;
        }
    }
}
