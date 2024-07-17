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

        public TokenController(ITokenUtils utils)
        {
            _utils = utils;
        }

        [HttpGet("token")]
        public IActionResult GetJwtToken()
        {

            var result = _utils.CreateToken(Guid.NewGuid().ToString());
            return Ok(result);
        }
    }
}