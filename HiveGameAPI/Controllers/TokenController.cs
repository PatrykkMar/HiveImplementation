using HiveGame.BusinessLogic.Models.Requests;
using HiveGame.BusinessLogic.Services;
using HiveGame.BusinessLogic.Utils;
using Microsoft.AspNetCore.Mvc;

namespace HiveGameAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TokenController : ControllerBase
    {
        //TODO: Result classes
        private readonly ITokenUtils _utils;
        private readonly IMatchmakingService _service;

        public TokenController(ITokenUtils utils, IMatchmakingService service)
        {
            _utils = utils;
            _service = service;
        }

        [HttpGet("token")]
        public IActionResult GetJwtToken()
        {
            var player = _service.CreatePlayer();
            var result = _utils.CreateToken("PlayerId" + player.PlayerId);
            return Ok(result);
        }

        [HttpGet("admin")]
        public IActionResult LoginAsAdmin([FromQuery] string password)
        {
            var result = _utils.CreateAdminToken(password);
            return Ok(result);
        }
    }
}