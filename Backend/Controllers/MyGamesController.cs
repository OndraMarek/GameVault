using GameVault.Data;
using GameVault.DTOs;
using GameVault.Models;
using GameVault.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameVault.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MyGamesController : ControllerBase
    {
        private readonly GameVaultContext _db;
        private readonly RawgService _rawgService;

        public MyGamesController(GameVaultContext db, RawgService rawgService)
        {
            _db = db;
            _rawgService = rawgService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? platform, [FromQuery] string? sortBy)
        {
            IQueryable<OwnedGame> query = _db.Games;

            if (!string.IsNullOrEmpty(platform) && platform != "All")
            {
                if (Enum.TryParse<GamingPlatform>(platform, out var requestedPlatform))
                {
                    query = query.Where(g => g.Platforms.Contains(requestedPlatform));
                }
            }

            if (!string.IsNullOrEmpty(sortBy))
            {
                if (sortBy == "TitleASC") query = query.OrderBy(g => g.Title);
                else if (sortBy == "TitleDESC") query = query.OrderByDescending(g => g.Title);
            }

            var gamesInMemory = await query.ToListAsync();

            var gamesDto = gamesInMemory.Select(game => new GameDetailDto(
                game.Id, game.RawgId, game.Title, game.Platforms.Select(p => p.ToString()).ToList(),
                game.HasPlayed, game.CoverImageUrl, game.Description, game.ReleaseDate, game.Genres, game.Developers)).ToList();

            return Ok(gamesDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var game = await _db.Games.FirstOrDefaultAsync(g => g.Id == id);
            if (game == null) return NotFound();

            if (string.IsNullOrEmpty(game.Description))
            {
                if (game.RawgId == null)
                {
                    var searchResponse = await _rawgService.SearchGameAsync(game.Title);
                    var bestMatch = searchResponse?.Results?.FirstOrDefault();

                    if (bestMatch != null)
                    {
                        game.RawgId = bestMatch.Id;
                        if (string.IsNullOrEmpty(game.CoverImageUrl)) game.CoverImageUrl = bestMatch.Background_image;
                    }
                }

                if (game.RawgId != null)
                {
                    var detailResponse = await _rawgService.GetGameDetailsAsync(game.RawgId.Value);
                    if (detailResponse != null)
                    {
                        game.Description = detailResponse.Description_raw;
                        game.ReleaseDate = detailResponse.Released;
                        game.Genres = detailResponse.Genres?.Select(g => g.Name).ToList() ?? new();
                        game.Developers = detailResponse.Developers?.Select(d => d.Name).ToList() ?? new();
                    }
                }
                await _db.SaveChangesAsync();
            }

            var dto = new GameDetailDto(
                game.Id, game.RawgId, game.Title, game.Platforms.Select(p => p.ToString()).ToList(),
                game.HasPlayed, game.CoverImageUrl, game.Description, game.ReleaseDate, game.Genres, game.Developers);

            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateGameDto dto)
        {
            int? rawgId = null;
            string? coverUrl = null, description = null, releaseDate = null;
            List<string> genres = new(), developers = new();

            var searchResponse = await _rawgService.SearchGameAsync(dto.Title);
            var bestMatch = searchResponse?.Results?.FirstOrDefault();

            if (bestMatch != null)
            {
                rawgId = bestMatch.Id;
                coverUrl = bestMatch.Background_image;

                var detailResponse = await _rawgService.GetGameDetailsAsync(rawgId.Value);
                if (detailResponse != null)
                {
                    description = detailResponse.Description_raw;
                    releaseDate = detailResponse.Released;
                    genres = detailResponse.Genres?.Select(g => g.Name).ToList() ?? new();
                    developers = detailResponse.Developers?.Select(d => d.Name).ToList() ?? new();
                }
            }

            OwnedGame newGame = new()
            {
                Id = Guid.NewGuid(),
                RawgId = rawgId,
                Title = dto.Title,
                Platforms = dto.Platforms,
                HasPlayed = dto.HasPlayed,
                CoverImageUrl = coverUrl,
                Description = description,
                ReleaseDate = releaseDate,
                Genres = genres,
                Developers = developers
            };

            _db.Games.Add(newGame);
            await _db.SaveChangesAsync();

            var responseDto = new GameDetailDto(
                newGame.Id, newGame.RawgId, newGame.Title, newGame.Platforms.Select(p => p.ToString()).ToList(),
                newGame.HasPlayed, newGame.CoverImageUrl, newGame.Description, newGame.ReleaseDate, newGame.Genres, newGame.Developers);

            return Ok(responseDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateGameDto dto)
        {
            var gameToUpdate = await _db.Games.FirstOrDefaultAsync(g => g.Id == id);
            if (gameToUpdate == null) return NotFound();

            gameToUpdate.RawgId = dto.RawgId;
            gameToUpdate.HasPlayed = dto.HasPlayed;
            gameToUpdate.Title = dto.Title;
            gameToUpdate.Platforms = dto.Platforms;
            gameToUpdate.CoverImageUrl = dto.CoverImageUrl;

            await _db.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var gameToDelete = await _db.Games.FirstOrDefaultAsync(g => g.Id == id);
            if (gameToDelete == null) return NotFound();

            _db.Games.Remove(gameToDelete);
            await _db.SaveChangesAsync();

            return Ok();
        }
    }
}