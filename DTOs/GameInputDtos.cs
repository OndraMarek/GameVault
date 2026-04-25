using GameVault.Models;

namespace GameVault.DTOs;

public record CreateGameDto(int RawgId, string Title, GamingPlatform Platform, int PlaytimeHours);

public record UpdateGameDto(int RawgId, string Title, GamingPlatform Platform, int PlaytimeHours);