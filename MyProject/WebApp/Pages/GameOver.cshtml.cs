using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace WebApp.Pages
{
    public class GameOver : PageModel
    {
        private readonly ILogger<GameOver> _logger;

        public GameOver(ILogger<GameOver> logger)
        {
            _logger = logger;
        }

        public string? Name { get; set; } = "Player";

        public void OnGet(string? name)
        {
            Name = name;
        }
    }
}