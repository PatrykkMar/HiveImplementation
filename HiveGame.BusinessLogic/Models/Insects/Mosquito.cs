using AutoMapper.Configuration.Annotations;
using HiveGame.BusinessLogic.Factories;
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
    public class Mosquito : Insect
    {
        public Mosquito(PlayerColor color)
        {
            Type = InsectType.Mosquito;
            PlayerColor = color;
        }

        public override InsectValidationResult GetAvailableVertices(IVertex moveFrom, IHiveBoard board)
        {
            var result = new InsectValidationResult();
            var factory = new InsectFactory();

            List<IVertex> vertices = BasicCheck(moveFrom, board, out string? whyMoveImpossible);

            if (vertices.Count == 0)
            {
                result.ReasonWhyEmpty = whyMoveImpossible;
                return result;
            }

            if (moveFrom.InsectStack.Count > 1)
                return factory.CreateInsect(InsectType.Beetle, PlayerColor).GetAvailableVertices(moveFrom, board);//mosquito being up works as beetle
            else
            {
                var insectsAround = board.GetAdjacentVerticesByCoordList(moveFrom)
                    .Where(x => !x.IsEmpty)
                    .Select(x => x.CurrentInsect.Type)
                    .ToArray();

                if (insectsAround.Length == 1 && insectsAround[0] == InsectType.Mosquito)
                {
                    result.ReasonWhyEmpty = "Mosquito can't move, it can't copy another mosquito";
                    return result;
                }

                var availableVertices = new List<IVertex>();

                foreach (var insect in insectsAround.Where(x=>!(new InsectType[] {InsectType.Mosquito, InsectType.Nothing}).Contains(x)))
                    availableVertices
                        .AddRange(factory.CreateInsect(insect, PlayerColor)
                            .GetAvailableVertices(moveFrom, board)
                            .AvailableVertices);

                availableVertices = availableVertices.Distinct().ToList();

                if (availableVertices.Count == 0)
                    result.ReasonWhyEmpty = "Mosquito as any insect to copy around cannot move";

                result.AvailableVertices = availableVertices;
                return result;
            }
        }
    }
}
