using HiveGame.BusinessLogic.Models.Graph;

namespace HiveGame.BusinessLogic.Models
{
    public class HiveActionResult
    {
        public HiveActionResult(Game game, BoardDTO board) 
        {
            CurrentBoard = board;
            Game = game;
        }
        public BoardDTO CurrentBoard { get; set; }
        public Game Game { get; set; }
        public bool GameOver { get; set; }
    }
}
