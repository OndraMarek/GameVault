using GameVault.Models;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

List<OwnedGame> myGames = [
    new OwnedGame(Guid.NewGuid(), 1942, "The Witcher 3: Wild Hunt", GamingPlatform.GOG, 120),
    new OwnedGame(Guid.NewGuid(), 732, "Grand Theft Auto V", GamingPlatform.Steam, 300),
    new OwnedGame(Guid.NewGuid(), 731, "Grand Theft Auto IV", GamingPlatform.Steam, 200),
    new OwnedGame(Guid.NewGuid(), 113, "Fortnite", GamingPlatform.EpicGames, 45),
];

app.MapGet("/api/mygames", () => myGames);

app.MapGet("/api/mygames/steam", () => myGames.Where(myGame => myGame.Platform == GamingPlatform.Steam));

app.MapGet("/api/mygames/{id}", (Guid id) => myGames.FirstOrDefault(myGame => myGame.Id == id));

app.MapPost("/api/mygames", (OwnedGame newGame) =>
{
    myGames.Add(newGame);

    return newGame;
});

app.MapDelete("/api/mygames/{id}", (Guid id) =>
{
    OwnedGame? gameToDelete = myGames.FirstOrDefault(myGame => myGame.Id == id);
    if (gameToDelete == null)
        return Results.NotFound();

    myGames.Remove(gameToDelete);

    return Results.Ok();
});

app.Run();