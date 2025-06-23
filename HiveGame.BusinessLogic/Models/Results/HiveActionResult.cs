using HiveGame.BusinessLogic.Models.Board;
using HiveGame.Core.Models;

namespace HiveGame.BusinessLogic.Models
{
    public class HiveActionResult
    {
        public bool GameOver { get; set; }
        public HiveActionResult(Game game) 
        {
            Game = game;
        }
        public Game Game { get; set; }
    }
}
