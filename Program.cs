using GameVault.Data;
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
    return await db.Games.ToListAsync();
});

app.MapGet("/api/mygames/{id}", async (GameVaultContext db, Guid id) =>
{
    return await db.Games.FirstOrDefaultAsync(myGame => myGame.Id == id);
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