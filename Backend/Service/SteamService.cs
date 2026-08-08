using GameVault.DTOs;

namespace GameVault.Services
{
    public class SteamService
    {
        private readonly HttpClient _httpClient;
        private readonly string _steamKey;

        public SteamService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _steamKey = configuration["SteamApiKey"] ?? throw new Exception("Steam key not found!");
        }

        public async Task<SteamApiResponse?> GetOwnedGamesAsync(string steamId)
        {
            string url = $"https://api.steampowered.com/IPlayerService/GetOwnedGames/v0001/?key={_steamKey}&steamid={steamId}&format=json&include_appinfo=true";
            return await _httpClient.GetFromJsonAsync<SteamApiResponse>(url);
        }
    }
}