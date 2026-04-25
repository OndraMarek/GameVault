using GameVault.Data;
using GameVault.DTOs;
using GameVault.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<GameVaultContext>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<GameVaultContext>();
    db.Database.EnsureCreated();
}

app.MapGet("/api/mygames", async (GameVaultContext db) =>
{
    var games = await db.Games
        .Select(game => new GameDetailDto(game.Title, game.Platform.ToString(), game.PlaytimeHours))
        .ToListAsync();

    return Results.Ok(games);
});

app.MapGet("/api/mygames/{id}", async (GameVaultContext db, Guid id) =>
{
    var game = await db.Games
        .Where(game => game.Id == id)
        .Select(game => new GameDetailDto(game.Title, game.Platform.ToString(), game.PlaytimeHours))
        .FirstOrDefaultAsync();

    if(game == null)
        return Results.NotFound();

    return Results.Ok(game);
});

app.MapPost("/api/mygames", async (OwnedGame newGame, GameVaultContext db) =>
{
    db.Games.Add(newGame);
    await db.SaveChangesAsync();

    return Results.Ok(newGame);
});

app.MapDelete("/api/mygames/{id}", async (Guid id, GameVaultContext db) =>
{
    OwnedGame? gameToDelete = db.Games.FirstOrDefault(myGame => myGame.Id == id);
    if (gameToDelete == null)
        return Results.NotFound();

    db.Games.Remove(gameToDelete);

    await db.SaveChangesAsync();

    return Results.Ok();
});

app.MapPut("/api/mygames/{id}", async (Guid id, OwnedGame updatedGame, GameVaultContext db) =>
{
    if (id != updatedGame.Id)
        return Results.BadRequest();

    OwnedGame? gameToUpdate = db.Games.FirstOrDefault(myGame => myGame.Id == id);
    if (gameToUpdate == null)
        return Results.NotFound();

    gameToUpdate.PlaytimeHours = updatedGame.PlaytimeHours;
    gameToUpdate.Title = updatedGame.Title;
    gameToUpdate.Platform = updatedGame.Platform;

    await db.SaveChangesAsync();

    return Results.Ok();
});

app.Run();