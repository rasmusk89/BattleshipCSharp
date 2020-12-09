using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.GamePlay
{
    public class Continue : PageModel
    {

        public int Id { get; set; }
        
        public void OnGet()
        {
            
        }

        public void OnPostAsync()
        {
            RedirectToPage("Index", new {newGame = false});
        }
    }
}