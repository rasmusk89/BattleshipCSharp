using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace WebApp.Pages
{
    public class GameOver : PageModel
    {
        private readonly ILogger<PrivacyModel> _logger;

        public GameOver(ILogger<PrivacyModel> logger)
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