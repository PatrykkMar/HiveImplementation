using HiveGame.BusinessLogic.Models.Board;
using HiveGame.Core.Models;

namespace HiveGame.BusinessLogic.Models
{
    public class DisconnectedWarningResult
    {
        public Game Game { get; set; }
        public Player DisconnectedPlayer { get; set; }
    }
}
