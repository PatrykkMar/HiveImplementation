using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveGame.BusinessLogic.Models.AuthModels
{
    public class TokenDatas
    {
        public string PlayerNick { get; set; }
        public long PlayerId { get; set; }
        public long? GameId { get; set; }

        public bool IsInTheGame
        {
            get
            {
                return GameId.HasValue;
            }
        }
    }
}
