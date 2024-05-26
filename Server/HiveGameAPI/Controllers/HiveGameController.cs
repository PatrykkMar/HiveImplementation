using HiveGame.BusinessLogic.Models.Requests;
using HiveGame.BusinessLogic.Services;
using Microsoft.AspNetCore.Mvc;

namespace HiveGameAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HiveGameController : ControllerBase
    {
        private readonly IHiveGameService _hiveGameService;

        public HiveGameController(IHiveGameService hiveGameService)
        {
            _hiveGameService = hiveGameService;
        }

        [HttpPost("move")]
        public IActionResult Move([FromBody] MoveRequest request)
        {
            var result = _hiveGameService.Move(request);
            return Ok(new { success = result });
        }

        [HttpPost("put")]
        public IActionResult Put([FromBody] PutRequest request)
        {
            _hiveGameService.Put(request);
            return Ok();
        }

        [HttpPost("putFirstInsect")]
        public IActionResult PutFirstInsect([FromBody] PutFirstInsectRequest request)
        {
            _hiveGameService.PutFirstInsect(request);
            return Ok();
        }
    }
}