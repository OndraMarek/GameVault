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

public class OwnedGame
{
    public Guid Id { get; set; }
    public int? RawgId { get; set; }
    public string Title { get; set; } = string.Empty;
    public List<GamingPlatform> Platforms { get; set; } = [];
    public int PlaytimeHours { get; set; }
    public string? CoverImageUrl { get; set; }
}