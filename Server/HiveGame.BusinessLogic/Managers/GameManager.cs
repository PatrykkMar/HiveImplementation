using HiveGame.BusinessLogic.Models;
using HiveGame.BusinessLogic.Models.Game;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;

namespace HiveGame.BusinessLogic.Managers
{
    public interface IGameManager
    {
        void AddGame(Game game);
    }

    public sealed class GameManager : IGameManager
    {
        private List<Game> _games = new();

        public void AddGame(Game game) 
        {
            _games.Add(game);
        }
    }
}
