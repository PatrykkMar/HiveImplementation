using HiveGame.BusinessLogic.Models;
using HiveGame.BusinessLogic.Models.Game;
using HiveGame.BusinessLogic.Models.Insects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveGame.BusinessLogic.Factories
{
    public interface IGameFactory
    {
        Game CreateGame(Player[] players);
    }
    public class GameFactory : IGameFactory
    {
        private readonly IInsectFactory _insectFactory;
        public GameFactory(IInsectFactory insectFactory)
        {
            _insectFactory = insectFactory;
        }

        public Game CreateGame(Player[] players)
        {
            var game = new Game(Guid.NewGuid().ToString(), players, _insectFactory);
            return game;
        }
    }
}
