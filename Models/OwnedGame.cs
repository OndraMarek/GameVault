namespace GameVault.Models;

public enum GamingPlatform
{
    Steam,
    EpicGames,
    PlayStation,
    Xbox,
    GOG,
    NintendoSwitch,
    UbisoftConnect,
    Other
}

public record OwnedGame(
    Guid Id,
    int IgdbId,
    string Title,
    GamingPlatform Platform,
    int PlaytimeHours
);