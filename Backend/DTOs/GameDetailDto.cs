namespace GameVault.DTOs;

public record GameDetailDto(Guid Id, int? RawgId, string Title, List<string> PlatformNames, bool HasPlayed, string? CoverImageUrl,
    string? Description, string? ReleaseDate, List<string> Genres, List<string> Developers);
