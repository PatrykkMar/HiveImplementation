using HiveGame.BusinessLogic.Models.Insects;
using HiveGame.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace HiveGame.BusinessLogic.Models
{
    public class Player
    {
        public string? PlayerId { get; set; }
        public PlayerColor PlayerColor { get; set; }
        public Dictionary<InsectType, int> PlayerInsects { get; set; }
        public int NumberOfMove { get; set; }

        public bool RemoveInsectFromPlayerBoard(InsectType insect)
        {
            if (PlayerInsects[insect] == 0)
                return false;

            PlayerInsects[insect]--;
            return true;
        }
    }
}
