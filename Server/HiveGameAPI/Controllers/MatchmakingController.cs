using HiveGame.BusinessLogic.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;

namespace Matchmaking.Controllers
{
    public class MatchmakingController : Controller
    {
        //TODO: Result classes
        private readonly IMatchmakingService _matchmakingService;

        public MatchmakingController(IMatchmakingService matchmakingService)
        {
            _matchmakingService = matchmakingService;
        }



        [HttpGet("join")]
        public async Task Get()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                var playerName = HttpContext.Request.Query["playerName"];
                var playerIP = HttpContext.Connection.RemoteIpAddress.ToString();

                await _matchmakingService.AddPlayerToQueueAsync(playerName, playerIP, webSocket);

                await HandleWebSocketCommunication(webSocket);
            }
            else
            {
                HttpContext.Response.StatusCode = 400;
            }
        }

        private async Task HandleWebSocketCommunication(WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            while (!result.CloseStatus.HasValue)
            {
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }
    }
}
