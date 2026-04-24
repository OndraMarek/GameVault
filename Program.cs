using GameVault.Data;
using GameVault.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<GameVaultContext>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<GameVaultContext>();
    db.Database.EnsureCreated();
}

app.MapGet("/api/mygames", (GameVaultContext db) =>
{
    return db.Games.ToList();
});

app.MapGet("/api/mygames/{id}", (GameVaultContext db, Guid id) =>
{
    return db.Games.FirstOrDefault(myGame => myGame.Id == id);
});

app.MapPost("/api/mygames", (OwnedGame newGame, GameVaultContext db) =>
{
    db.Games.Add(newGame);
    db.SaveChanges();

    return Results.Ok(newGame);
});

app.MapDelete("/api/mygames/{id}", (Guid id, GameVaultContext db) =>
{
    OwnedGame? gameToDelete = db.Games.FirstOrDefault(myGame => myGame.Id == id);
    if (gameToDelete == null)
        return Results.NotFound();

    db.Games.Remove(gameToDelete);

    db.SaveChanges();

    return Results.Ok();
});

app.MapPut("/api/mygames/{id}", (Guid id, OwnedGame updatedGame, GameVaultContext db) =>
{
    if (id != updatedGame.Id)
        return Results.BadRequest();

    OwnedGame? gameToUpdate = db.Games.FirstOrDefault(myGame => myGame.Id == id);
    if (gameToUpdate == null)
        return Results.NotFound();

    gameToUpdate.PlaytimeHours = updatedGame.PlaytimeHours;
    gameToUpdate.Title = updatedGame.Title;
    gameToUpdate.Platform = updatedGame.Platform;

    db.SaveChanges();

    return Results.Ok();
});

app.Run();