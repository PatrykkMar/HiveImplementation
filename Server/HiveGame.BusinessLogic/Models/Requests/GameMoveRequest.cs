using HiveGame.BusinessLogic.Models.AuthModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveGame.BusinessLogic.Models.Requests
{
    public class GameMoveRequest
    {
        public long PlayerId { get; set; }
        public long? GameId { get; set; }

        public void ReadDataFromJWT(TokenDatas tokenDatas)
        {
            PlayerId = tokenDatas.PlayerId;
            GameId = tokenDatas.GameId;
        }
    }
}
