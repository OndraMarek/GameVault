namespace GameVault.DTOs;

public class RawgSearchResponse
{
    public List<RawgGameDto> Results { get; set; } = [];
}

public class RawgGameDetailResponse
{
    public string Description_raw { get; set; } = string.Empty;
    public string Released { get; set; } = string.Empty;
    public List<RawgItemDto> Developers { get; set; } = [];
    public List<RawgItemDto> Genres { get; set; } = [];
}

public class RawgItemDto
{
    public string Name { get; set; } = string.Empty;
}

public class RawgGameDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Background_image { get; set; } = string.Empty;
    public string Released { get; set; } = string.Empty;
}