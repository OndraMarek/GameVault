namespace GameVault.DTOs;

public class RawgSearchResponse
{
    public List<RawgGameDto> Results { get; set; } = [];
}

public class RawgGameDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Background_image { get; set; } = string.Empty;
    public string Released { get; set; } = string.Empty;
}