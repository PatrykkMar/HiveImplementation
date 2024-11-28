using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveGame.BusinessLogic.Models.WebSocketModels
{
    [Serializable]
    public class WebSocketMessage
    {
        public string Message { get; set; }
        public MessageType Type { get; set; }
    }

    public enum MessageType
    {
        GameCreated
    }
}
