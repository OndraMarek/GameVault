using GameVault.Data;
using GameVault.DTOs;
using GameVault.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

string frontendUrl = builder.Configuration["FrontendUrl"] ?? "http://localhost:5173";

string apiKey = builder.Configuration["RawgApiKey"]
    ?? throw new Exception("API key not found!");

string steamKey = builder.Configuration["SteamApiKey"]
    ?? throw new Exception("Steam key not found!");

builder.Services.AddDbContext<GameVaultContext>();
builder.Services.AddHttpClient();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact", builder =>
    {
        builder.WithOrigins(frontendUrl)
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
});

var app = builder.Build();

app.UseCors("AllowReact");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<GameVaultContext>();
    db.Database.EnsureCreated();
}

app.MapGet("/api/mygames", async (GameVaultContext db) =>
{
    var gamesInMemory = await db.Games.ToListAsync();
    var gamesDto = gamesInMemory
        .Select(game => new GameDetailDto(game.Id, game.RawgId, game.Title, game.Platforms.Select(p => p.ToString()).ToList(), game.PlaytimeHours, game.CoverImageUrl))
        .ToList();

    return Results.Ok(gamesDto);
});

app.MapGet("/api/mygames/{id}", async (GameVaultContext db, Guid id) =>
{
    var gameInMemory = await db.Games.FirstOrDefaultAsync(g => g.Id == id);

    if (gameInMemory == null)
        return Results.NotFound();

    var game = new GameDetailDto(gameInMemory.Id, gameInMemory.RawgId, gameInMemory.Title, gameInMemory.Platforms.Select(p => p.ToString()).ToList(), gameInMemory.PlaytimeHours, gameInMemory.CoverImageUrl);

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
        Platforms = dto.Platforms,
        PlaytimeHours = dto.PlaytimeHours,
        CoverImageUrl = dto.CoverImageUrl
    };

    db.Games.Add(newGame);
    await db.SaveChangesAsync();

    var responseDto = new GameDetailDto(newGame.Id, newGame.RawgId, newGame.Title, newGame.Platforms.Select(p => p.ToString()).ToList(), newGame.PlaytimeHours, newGame.CoverImageUrl);

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
    gameToUpdate.Platforms = dto.Platforms;
    gameToUpdate.CoverImageUrl = dto.CoverImageUrl;

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
            bool gameExists = db.Games.Any(g => g.Title == steamGame.Name && g.Platforms.Contains(GamingPlatform.Steam));

            if (!gameExists)
            {
                OwnedGame? newGame = new()
                {
                    Id = Guid.NewGuid(),
                    RawgId = null,
                    Title = steamGame.Name,
                    Platforms = [GamingPlatform.Steam],
                    PlaytimeHours = steamGame.Playtime_forever / 60,
                    CoverImageUrl = null
                };
                db.Games.Add(newGame);
            }
        }
    }

    await db.SaveChangesAsync();

    return Results.Ok("Synchronization was successful!");
});

app.Run();