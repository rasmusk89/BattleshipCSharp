using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Confirmation
{
    public class Index : PageModel
    {
        public string Message { get; set; } = "";

        public string PlayerName { get; set; } = "";
        public string OpponentName { get; set; } = "";

        [BindProperty] public int Id { get; set; }

        public void OnGetAsync(int id, string? message, string player, string opponent)
        {
            if (message != null)
            {
                Message = message!;
            }

            PlayerName = player;
            OpponentName = opponent;
            Id = id;
        }
    }
}