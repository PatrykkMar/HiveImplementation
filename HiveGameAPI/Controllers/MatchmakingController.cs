using HiveGame.BusinessLogic.Models;
using HiveGame.BusinessLogic.Repositories;
using HiveGame.BusinessLogic.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace HiveGameAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = Roles.Admin)]
    public class MatchmakingController : ControllerBase
    {
        private readonly IMatchmakingRepository _matchmakingRepository;

        public MatchmakingController(IMatchmakingRepository repository)
        {
            _matchmakingRepository = repository;
        }

        // GET: /Matchmaking
        [HttpGet]
        public ActionResult<IEnumerable<Player>> GetAllPlayers()
        {
            var players = _matchmakingRepository.GetAll();
            return Ok(players);
        }

        // GET: /Matchmaking/{playerId}
        [HttpGet("{playerId}")]
        public ActionResult<Player> GetPlayerById(string playerId)
        {
            var player = _matchmakingRepository.GetByPlayerId(playerId);

            if (player == null)
            {
                return NotFound($"Player with ID {playerId} not found.");
            }

            return Ok(player);
        }

        // POST: /Matchmaking
        [HttpPost]
        public ActionResult AddPlayer([FromBody] Player player)
        {
            if (player == null)
            {
                return BadRequest("Invalid player data.");
            }

            _matchmakingRepository.Add(player);
            return CreatedAtAction(nameof(GetPlayerById), new { playerId = player.PlayerId }, player);
        }

        // DELETE: /Matchmaking/{playerId}
        [HttpDelete("{playerId}")]
        public ActionResult DeletePlayer(string playerId)
        {
            var success = _matchmakingRepository.Remove(playerId);

            if (!success)
            {
                return NotFound($"Player with ID {playerId} not found.");
            }

            return NoContent();
        }
    }
}