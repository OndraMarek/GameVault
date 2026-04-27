namespace GameVault.DTOs;

public class SteamApiResponse
{
    public SteamResponseData Response { get; set; } = new();
}

public class SteamResponseData
{
    public int Game_count { get; set; }
    public List<SteamGameDto> Games { get; set; } = [];
}

public class SteamGameDto
{
    public int Appid { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Playtime_forever { get; set; }
}