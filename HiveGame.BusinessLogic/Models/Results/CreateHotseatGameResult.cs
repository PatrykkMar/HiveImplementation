using HiveGame.BusinessLogic.Models.Board;
using HiveGame.Core.Models;

namespace HiveGame.BusinessLogic.Models
{
    public class CreateHotseatGameResult
    {
        public Player ExistingPlayer { get; set; }
        public Player AddedNewPlayer { get; set; }
        public Game Game { get; set; }
    }
}
