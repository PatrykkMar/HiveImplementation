using HiveGame.BusinessLogic.Services;
using Microsoft.AspNetCore.Mvc;

namespace HiveGame.Controllers
{
    public class MatchmakingController : Controller
    {
        //TODO: Result classes
        private readonly IHiveGameService _hiveGameService;

        public MatchmakingController(IHiveGameService hiveGameService)
        {
            _hiveGameService = hiveGameService;
        }

        [HttpGet("join")]
        public IActionResult JoinTheGame()
        {
            throw new NotImplementedException();
        }
    }
}
