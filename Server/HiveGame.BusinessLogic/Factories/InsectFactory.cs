using HiveGame.BusinessLogic.Models;
using HiveGame.BusinessLogic.Models.Insects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveGame.BusinessLogic.Factories
{
    public interface IInsectFactory
    {
        Insect CreateInsect(InsectType insectType, PlayerColor playerColor);
    }
    public class InsectFactory : IInsectFactory
    {
        public Insect CreateInsect(InsectType insectType, PlayerColor playerColor)
        {
            switch (insectType)
            {
                case InsectType.Queen:
                    return new Queen(playerColor);
                case InsectType.Ant:
                    return new Ant(playerColor);
                case InsectType.Spider:
                    return new Spider(playerColor);
                case InsectType.Grasshopperm:
                    return new Grasshopperm(playerColor);
                case InsectType.Beetle:
                    return new Beetle(playerColor);
                default:
                    throw new ArgumentException("Unknown insect type.");
            }
        }
    }
}
