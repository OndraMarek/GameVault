using GameVault.DTOs;

namespace GameVault.Services
{
    public class RawgService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public RawgService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["RawgApiKey"] ?? throw new Exception("API key not found!");
        }

        public async Task<RawgSearchResponse?> SearchGameAsync(string title)
        {
            return await _httpClient.GetFromJsonAsync<RawgSearchResponse>($"https://api.rawg.io/api/games?key={_apiKey}&search={title}");
        }

        public async Task<RawgGameDetailResponse?> GetGameDetailsAsync(int rawgId)
        {
            return await _httpClient.GetFromJsonAsync<RawgGameDetailResponse>($"https://api.rawg.io/api/games/{rawgId}?key={_apiKey}");
        }
    }
}