using GameVault.Data;
using GameVault.Models;
using GameVault.Services;
using Microsoft.AspNetCore.Mvc;

namespace GameVault.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SyncController : ControllerBase
    {
        private readonly GameVaultContext _db;
        private readonly SteamService _steamService;

        public SyncController(GameVaultContext db, SteamService steamService)
        {
            _db = db;
            _steamService = steamService;
        }

        [HttpPost("steam/{steamId}")]
        public async Task<IActionResult> SyncSteam(string steamId)
        {
            var result = await _steamService.GetOwnedGamesAsync(steamId);

            if (result?.Response?.Games != null)
            {
                foreach (var steamGame in result.Response.Games)
                {
                    bool gameExists = _db.Games.Any(g => g.Title == steamGame.Name && g.Platforms.Contains(GamingPlatform.Steam));

                    if (!gameExists)
                    {
                        OwnedGame newGame = new()
                        {
                            Id = Guid.NewGuid(),
                            Title = steamGame.Name,
                            Platforms = new List<GamingPlatform> { GamingPlatform.Steam },
                            HasPlayed = steamGame.Playtime_forever > 0,
                            CoverImageUrl = $"https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/{steamGame.Appid}/library_600x900_2x.jpg",
                            Genres = new(),
                            Developers = new()
                        };
                        _db.Games.Add(newGame);
                    }
                    else
                    {
                        var existingGame = _db.Games.First(g => g.Title == steamGame.Name && g.Platforms.Contains(GamingPlatform.Steam));
                        bool isPlayedOnSteam = steamGame.Playtime_forever > 0;

                        if (existingGame.HasPlayed != isPlayedOnSteam || string.IsNullOrEmpty(existingGame.CoverImageUrl))
                        {
                            existingGame.HasPlayed = isPlayedOnSteam;

                            if (string.IsNullOrEmpty(existingGame.CoverImageUrl))
                            {
                                existingGame.CoverImageUrl = $"https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/{steamGame.Appid}/library_600x900_2x.jpg";
                            }
                        }
                    }
                }
                await _db.SaveChangesAsync();
            }

            return Ok("Synchronization was successful!");
        }
    }
}