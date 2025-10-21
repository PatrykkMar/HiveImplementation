using HiveGame.BusinessLogic.Models.Board;
using HiveGame.Core.Models;

namespace HiveGame.BusinessLogic.Models
{
    public class JoinQueueResult
    {
        public Player[]? PendingPlayers { get; set; }
        public Player? Player { get; set; }
    }
}
