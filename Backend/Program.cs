using GameVault.Data;
using GameVault.DTOs;
using GameVault.Models;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;
using System.Runtime.InteropServices;


var builder = WebApplication.CreateBuilder(args);

string apiKey = builder.Configuration["RawgApiKey"]
    ?? throw new Exception("API key not found!");

string steamKey = builder.Configuration["SteamApiKey"]
    ?? throw new Exception("Steam key not found!");

builder.Services.AddDbContext<GameVaultContext>();
builder.Services.AddHttpClient();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<GameVaultContext>();
    db.Database.EnsureCreated();
}

app.MapGet("/api/mygames", async (GameVaultContext db) =>
{
    var games = await db.Games
        .Select(game => new GameDetailDto(game.Id, game.Title, game.Platform.ToString(), game.PlaytimeHours))
        .ToListAsync();

    return Results.Ok(games);
});

app.MapGet("/api/mygames/{id}", async (GameVaultContext db, Guid id) =>
{
    var game = await db.Games
        .Where(game => game.Id == id)
        .Select(game => new GameDetailDto(game.Id, game.Title, game.Platform.ToString(), game.PlaytimeHours))
        .FirstOrDefaultAsync();

    if (game == null)
        return Results.NotFound();

    return Results.Ok(game);
});

app.MapGet("/api/search/{title}", async (string title, IHttpClientFactory factory) =>
{
    HttpClient client = factory.CreateClient();
    var result = await client.GetFromJsonAsync<RawgSearchResponse>($"https://api.rawg.io/api/games?key={apiKey}&search={title}");

    return Results.Ok(result?.Results);
});

app.MapPost("/api/mygames", async (CreateGameDto dto, GameVaultContext db) =>
{
    OwnedGame newGame = new()
    {
        Id = Guid.NewGuid(),
        RawgId = dto.RawgId,
        Title = dto.Title,
        Platform = dto.Platform,
        PlaytimeHours = dto.PlaytimeHours
    };

    db.Games.Add(newGame);
    await db.SaveChangesAsync();

    var responseDto = new GameDetailDto(newGame.Id, newGame.Title, newGame.Platform.ToString(), newGame.PlaytimeHours);

    return Results.Ok(responseDto);
});

app.MapDelete("/api/mygames/{id}", async (Guid id, GameVaultContext db) =>
{
    OwnedGame? gameToDelete = await db.Games.FirstOrDefaultAsync(myGame => myGame.Id == id);
    if (gameToDelete == null)
        return Results.NotFound();

    db.Games.Remove(gameToDelete);

    await db.SaveChangesAsync();

    return Results.Ok();
});

app.MapPut("/api/mygames/{id}", async (Guid id, UpdateGameDto dto, GameVaultContext db) =>
{
    OwnedGame? gameToUpdate = await db.Games.FirstOrDefaultAsync(myGame => myGame.Id == id);

    if (gameToUpdate == null)
        return Results.NotFound();

    gameToUpdate.RawgId = dto.RawgId;
    gameToUpdate.PlaytimeHours = dto.PlaytimeHours;
    gameToUpdate.Title = dto.Title;
    gameToUpdate.Platform = dto.Platform;

    await db.SaveChangesAsync();

    return Results.Ok();
});

app.MapPost("/api/sync/steam/{steamId}", async (string steamId, IHttpClientFactory factory, GameVaultContext db) =>
{
    var client = factory.CreateClient();

    string url = $"https://api.steampowered.com/IPlayerService/GetOwnedGames/v0001/?key={steamKey}&steamid={steamId}&format=json&include_appinfo=true";

    var result = await client.GetFromJsonAsync<SteamApiResponse>(url);

    if (result?.Response?.Games != null)
    {
        foreach (var steamGame in result.Response.Games)
        {
            bool gameExists = db.Games.Any(g => g.Title == steamGame.Name && g.Platform == GamingPlatform.Steam);

            if (!gameExists)
            {
                OwnedGame? newGame = new()
                {
                    Id = Guid.NewGuid(),
                    RawgId = null,
                    Title = steamGame.Name,
                    Platform = GamingPlatform.Steam,
                    PlaytimeHours = steamGame.Playtime_forever / 60
                };
                db.Games.Add(newGame);
            }
        }
    }

    await db.SaveChangesAsync();

    return Results.Ok("Synchronization was successful!");
});

app.Run();