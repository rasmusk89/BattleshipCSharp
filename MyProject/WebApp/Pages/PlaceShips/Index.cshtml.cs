using System.Linq;
using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Player = GameBrain.Player;

namespace WebApp.Pages.PlaceShips
{
    
    public class Index : PageModel
    {
        
        private readonly ILogger<IndexModel> _logger;
        private readonly AppDbContext _context;


        public Index(ILogger<IndexModel> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public Player? PlayerA { get; set; }
        
        public void OnGet(Player player)
        {

            PlayerA = player;

        }
    }
}