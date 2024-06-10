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
        Insect CreateInsect(InsectType insectType);
    }
    public class InsectFactory : IInsectFactory
    {
        public Insect CreateInsect(InsectType insectType)
        {
            switch (insectType)
            {
                case InsectType.Queen:
                    return new Queen();
                case InsectType.Ant:
                    return new Ant();
                case InsectType.Spider:
                    return new Spider();
                case InsectType.Grasshopperm:
                    return new Grasshopperm();
                case InsectType.Beetle:
                    return new Beetle();
                default:
                    throw new ArgumentException("Unknown insect type.");
            }
        }
    }
}
