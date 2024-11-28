using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace HiveGame.Core.Models
{
    public class PlayerViewDTO
    {
        public BoardDTO Board { get; set; }

        public Dictionary<InsectType, int> PlayerInsects { get; set; }
    }
}
