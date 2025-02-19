using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveGame.Core.Models.Requests
{
    public class ReceiveMessageRequest
    {
        public ReceiveMessageRequest(string playerId, string message, ClientState? state, PlayerViewDTO playerView)
        {
            this.playerId = playerId;
            this.message = message;
            this.state = state;
            this.playerView = playerView;
        }

        public string playerId { get; set; }
        public string message { get; set; }
        public ClientState? state { get; set; }
        public PlayerViewDTO playerView { get; set; }
    }
}
