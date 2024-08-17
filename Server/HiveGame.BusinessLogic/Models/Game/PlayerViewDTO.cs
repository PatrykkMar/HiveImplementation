using HiveGame.BusinessLogic.Factories;
using HiveGame.BusinessLogic.Models.Game.Graph;
using HiveGame.BusinessLogic.Models.Graph;
using HiveGame.BusinessLogic.Models.Insects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace HiveGame.BusinessLogic.Models.Game
{
    [Serializable]
    public class PlayerViewDTO
    {
        public List<VertexDTO> Board { get; set; }

        public Dictionary<InsectType, int> PlayerInsects { get; set; }
    }
}
