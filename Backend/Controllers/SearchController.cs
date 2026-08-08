using GameVault.Services;
using Microsoft.AspNetCore.Mvc;

namespace GameVault.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly RawgService _rawgService;

        public SearchController(RawgService rawgService)
        {
            _rawgService = rawgService;
        }

        [HttpGet("{title}")]
        public async Task<IActionResult> SearchByTitle(string title)
        {
            var result = await _rawgService.SearchGameAsync(title);
            return Ok(result?.Results);
        }
    }
}