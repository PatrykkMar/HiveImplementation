using HiveGame.BusinessLogic.Models.Graph;

namespace HiveGame.BusinessLogic.Models.Game
{
    public class HiveActionResult
    {
        public HiveActionResult(Game game, List<VertexDTO> vertices) 
        {
            CurrentBoard = vertices;
            Game = game;
        }
        public List<VertexDTO> CurrentBoard { get; set; }
        public Game Game { get; set; }
    }
}
