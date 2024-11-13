using AutoMapper;
using HiveGame.BusinessLogic.Models;
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
        public GameFactory()
        {
        }

        public Game CreateGame(Player[] players)
        {
            var game = new Game(players);

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
                    else if(new InsectType[] { InsectType.Spider, InsectType.Beetle }.Contains(insect))
                        player.PlayerInsects.Add(insect, 2);
                    else
                        player.PlayerInsects.Add(insect, 3);
                }
            }

            return game;
        }
    }
}
