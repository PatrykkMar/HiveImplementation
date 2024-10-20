using AutoMapper;
using HiveGame.BusinessLogic.Models;
using HiveGame.BusinessLogic.Models;
using HiveGame.BusinessLogic.Models.Graph;
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
        private readonly IMapper _mapper;
        public GameFactory(IInsectFactory insectFactory, IMapper mapper)
        {
            _insectFactory = insectFactory;
            _mapper = mapper;
        }

        public Game CreateGame(Player[] players)
        {
            var game = new Game(players, _insectFactory, _mapper);

            foreach( var player in game.Players) 
            {
                player.PlayerInsects = new Dictionary<InsectType, int>();
            }

            foreach( var insect in (InsectType[])Enum.GetValues(typeof(InsectType))) 
            {
                foreach(var player in game.Players)
                {
                    if(insect == InsectType.Queen)
                        player.PlayerInsects.Add(insect, 1);
                    else
                        player.PlayerInsects.Add(insect, 2);
                }
            }

            return game;
        }
    }
}
