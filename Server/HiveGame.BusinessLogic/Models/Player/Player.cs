﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace HiveGame.BusinessLogic.Models
{
    public class Player
    {
        public string Nick { get; set; }
        public string IP { get; set; }
        public WebSocket WebSocket { get; set; }
    }
}
