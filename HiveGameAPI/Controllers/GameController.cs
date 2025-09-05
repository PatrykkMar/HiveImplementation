using HiveGame.BusinessLogic.Models;
using HiveGame.BusinessLogic.Models.Requests;
using HiveGame.BusinessLogic.Repositories;
using HiveGame.BusinessLogic.Utils;
using HiveGame.DataAccess.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data;

namespace HiveGameAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = Roles.Admin)]
    public class GameController : ControllerBase
    {
        private readonly IGameRepository _gameRepository;
        private readonly IGameConverter _converter;

        public GameController(IGameRepository repository, IGameConverter converter)
        {
            _gameRepository = repository;
            _converter = converter;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Game>> GetAllGames()
        {
            var games = _gameRepository.GetAllAsync();
            return Ok(games);
        }

        // GET: /Game/{gameId}
        [HttpGet("{gameId}")]
        public async Task<ActionResult> GetGameById(string gameId)
        {
            var game = await _gameRepository.GetByGameIdAsync(gameId);

            if (game == null)
            {
                return NotFound($"Game with ID {gameId} not found.");
            }

            return Ok(game);
        }

        // GET: /Game/player/{playerId}
        [HttpGet("player/{playerId}")]
        public ActionResult<Game> GetGameByPlayerId(string playerId)
        {
            var game = _gameRepository.GetByPlayerIdAsync(playerId);

            if (game == null)
            {
                return NotFound($"No game found for player with ID {playerId}.");
            }

            return Ok(game);
        }

        // POST: /Game
        [HttpPost]
        public async Task<ActionResult> AddGame([FromBody] Game game)
        {
            if (game == null)
            {
                return BadRequest("Invalid game data.");
            }

            await _gameRepository.AddAsync(_converter.ToGameDbModel(game));

            return CreatedAtAction(nameof(GetGameById), new { gameId = game.Id }, game);
        }

        // DELETE: /Game/{gameId}
        [HttpDelete("{gameId}")]
        public async Task<ActionResult> DeleteGame(string gameId)
        {
            var success = await _gameRepository.RemoveAsync(gameId);

            if (!success)
            {
                return NotFound($"Game with ID {gameId} not found.");
            }

            return NoContent();
        }
    }
}