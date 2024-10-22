using HiveGame.BusinessLogic.Factories;
using HiveGame.BusinessLogic.Models.Graph;
using HiveGame.BusinessLogic.Models.Graph;
using HiveGame.BusinessLogic.Models.Insects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace HiveGame.BusinessLogic.Models
{
    public class PlayerViewDTO
    {
        public BoardDTO Board { get; set; }

        public Dictionary<InsectType, int> PlayerInsects { get; set; }
    }
}
