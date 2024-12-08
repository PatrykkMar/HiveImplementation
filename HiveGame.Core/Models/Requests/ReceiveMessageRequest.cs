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
            PlayerId = playerId;
            Message = message;
            State = state;
            PlayerView = playerView;
        }

        public string PlayerId { get; set; }
        public string Message { get; set; }
        public ClientState? State { get; set; }
        public PlayerViewDTO PlayerView { get; set; }
    }
}
