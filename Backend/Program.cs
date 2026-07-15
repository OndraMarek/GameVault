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

app.MapGet("/api/mygames", async (GameVaultContext db, string? platform, string? sortBy) =>
{
    IQueryable<OwnedGame> query = db.Games;

    if (!string.IsNullOrEmpty(platform) && platform != "All")
    {
        if (Enum.TryParse<GamingPlatform>(platform, out var requestedPlatform))
        {
            query = query.Where(g => g.Platforms.Contains(requestedPlatform));
        }
    }

    if (!string.IsNullOrEmpty(sortBy))
    {
        if (sortBy == "TitleASC")
        {
            query = query.OrderBy(g => g.Title);
        }
        else if (sortBy == "TitleDESC")
        {
            query = query.OrderByDescending(g => g.Title);
        }
    }

    var gamesInMemory = await query.ToListAsync();

    var gamesDto = gamesInMemory
        .Select(game => new GameDetailDto(
            game.Id,
            game.RawgId,
            game.Title,
            game.Platforms.Select(p => p.ToString()).ToList(),
            game.HasPlayed,
            game.CoverImageUrl,
            game.Description,
            game.ReleaseDate,
            game.Genres,
            game.Developers))
        .ToList();

    return Results.Ok(gamesDto);
});

app.MapGet("/api/mygames/{id}", async (GameVaultContext db, Guid id, IHttpClientFactory factory) =>
{
    var game = await db.Games.FirstOrDefaultAsync(g => g.Id == id);

    if (game == null)
        return Results.NotFound();

    if (string.IsNullOrEmpty(game.Description))
    {
        var client = factory.CreateClient();

        if (game.RawgId == null)
        {
            var searchResponse = await client.GetFromJsonAsync<RawgSearchResponse>($"https://api.rawg.io/api/games?key={apiKey}&search={game.Title}");
            var bestMatch = searchResponse?.Results?.FirstOrDefault();

            if (bestMatch != null)
            {
                game.RawgId = bestMatch.Id;
                if (string.IsNullOrEmpty(game.CoverImageUrl))
                {
                    game.CoverImageUrl = bestMatch.Background_image;
                }
            }
        }

        if (game.RawgId != null)
        {
            var detailResponse = await client.GetFromJsonAsync<RawgGameDetailResponse>($"https://api.rawg.io/api/games/{game.RawgId}?key={apiKey}");

            if (detailResponse != null)
            {
                game.Description = detailResponse.Description_raw;
                game.ReleaseDate = detailResponse.Released;
                game.Genres = detailResponse.Genres?.Select(g => g.Name).ToList() ?? [];
                game.Developers = detailResponse.Developers?.Select(d => d.Name).ToList() ?? [];
            }
        }

        await db.SaveChangesAsync();
    }

    var dto = new GameDetailDto(
        game.Id,
        game.RawgId,
        game.Title,
        game.Platforms.Select(p => p.ToString()).ToList(),
        game.HasPlayed,
        game.CoverImageUrl,
        game.Description,
        game.ReleaseDate,
        game.Genres,
        game.Developers);

    return Results.Ok(dto);
});

app.MapGet("/api/search/{title}", async (string title, IHttpClientFactory factory) =>
{
    HttpClient client = factory.CreateClient();
    var result = await client.GetFromJsonAsync<RawgSearchResponse>($"https://api.rawg.io/api/games?key={apiKey}&search={title}");

    return Results.Ok(result?.Results);
});

app.MapPost("/api/mygames", async (CreateGameDto dto, GameVaultContext db, IHttpClientFactory factory) =>
{
    var client = factory.CreateClient();

    int? rawgId = null;
    string? coverUrl = null;
    string? description = null;
    string? releaseDate = null;
    List<string> genres = [];
    List<string> developers = [];

    var searchResponse = await client.GetFromJsonAsync<RawgSearchResponse>($"https://api.rawg.io/api/games?key={apiKey}&search={dto.Title}");
    var bestMatch = searchResponse?.Results.FirstOrDefault();

    if (bestMatch != null)
    {
        rawgId = bestMatch.Id;
        coverUrl = bestMatch.Background_image;

        var detailResponse = await client.GetFromJsonAsync<RawgGameDetailResponse>($"https://api.rawg.io/api/games/{rawgId}?key={apiKey}");
        if (detailResponse != null)
        {
            description = detailResponse.Description_raw;
            releaseDate = detailResponse.Released;
            genres = detailResponse.Genres?.Select(g => g.Name).ToList() ?? [];
            developers = detailResponse.Developers?.Select(d => d.Name).ToList() ?? [];
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

    db.Games.Add(newGame);
    await db.SaveChangesAsync();

    var responseDto = new GameDetailDto(
        newGame.Id,
        newGame.RawgId,
        newGame.Title,
        newGame.Platforms.Select(p => p.ToString()).ToList(),
        newGame.HasPlayed,
        newGame.CoverImageUrl,
        newGame.Description,
        newGame.ReleaseDate,
        newGame.Genres,
        newGame.Developers);

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
    gameToUpdate.HasPlayed = dto.HasPlayed;
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
                    HasPlayed = steamGame.Playtime_forever > 0,
                    CoverImageUrl = $"https://shared.akamai.steamstatic.com/store_item_assets/steam/apps/{steamGame.Appid}/library_600x900_2x.jpg",
                    Description = null,
                    ReleaseDate = null,
                    Genres = [],
                    Developers = []
                };
                db.Games.Add(newGame);
            }
            else
            {
                var existingGame = db.Games.First(g => g.Title == steamGame.Name && g.Platforms.Contains(GamingPlatform.Steam));
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
    }

    await db.SaveChangesAsync();

    return Results.Ok("Synchronization was successful!");
});

app.Run();