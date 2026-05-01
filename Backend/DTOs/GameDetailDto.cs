namespace GameVault.DTOs;

public record GameDetailDto(Guid Id,string Title, List<string> PlatformNames, int Playtime, string? CoverImageUrl);
