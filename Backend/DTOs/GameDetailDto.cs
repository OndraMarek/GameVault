namespace GameVault.DTOs;

public record GameDetailDto(Guid Id, int? RawgId, string Title, List<string> PlatformNames, bool HasPlayed, string? CoverImageUrl);
