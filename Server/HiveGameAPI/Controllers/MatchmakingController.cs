using HiveGame.BusinessLogic.Models.Requests;
using HiveGame.BusinessLogic.Services;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> JoinQueue([FromQuery] string nick)
        {
            var request = new AddToQueueRequest();
            request.Nick = nick;
            var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            await _matchmakingService.JoinQueue(request.Nick, webSocket);
            return Ok();
        }
    }
}
