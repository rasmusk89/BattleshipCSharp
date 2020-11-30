using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace WebApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly DAL.AppDbContext _context;


        public IndexModel(ILogger<IndexModel> logger, DAL.AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [BindProperty] public Player? PlayerA { get; set; }

        [BindProperty] public Player? PlayerB { get; set; }

        [BindProperty] public GameOption? GameOption { get; set; }

        public List<Game>? Games { get; set; }

        public IActionResult OnGet()
        {
            Games = _context.Games.OrderByDescending(id => id.GameId).ToList();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var game = new Game
            {
                Description = PlayerA!.Name + "_" + PlayerB!.Name + "_" + DateTime.Now,
                PlayerA = new Player {Name = PlayerA!.Name, PlayerType = PlayerA!.PlayerType},
                PlayerB = new Player {Name = PlayerB!.Name, PlayerType = PlayerB!.PlayerType},
                GameOption = new GameOption()
                {
                    Name = DateTime.Now.ToString(CultureInfo.InvariantCulture),
                    BoardWidth = GameOption!.BoardWidth,
                    BoardHeight = GameOption!.BoardHeight,
                    EShipsCanTouch = GameOption!.EShipsCanTouch,
                    NextMoveAfterHit = GameOption!.NextMoveAfterHit
                }
            };

            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _context.Games.AddAsync(game);
            await _context.SaveChangesAsync();
            
            return RedirectToPage("./GamePlay/Index");
        }
    }
}