using GameVault.Models;

namespace GameVault.DTOs;

public record CreateGameDto(int? RawgId, string Title, List<GamingPlatform> Platforms, int PlaytimeHours, string? CoverImageUrl);

public record UpdateGameDto(int? RawgId, string Title, List<GamingPlatform> Platforms, int PlaytimeHours, string? CoverImageUrl);