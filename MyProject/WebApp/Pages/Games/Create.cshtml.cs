using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using DAL;
using Domain;

namespace WebApp.Pages_Games
{
    public class CreateModel : PageModel
    {
        private readonly DAL.AppDbContext _context;

        public CreateModel(DAL.AppDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            ViewData["Description"] = "Hello!";
            ViewData["GameOptionId"] = new SelectList(_context.GameOptions, "GameOptionId", "Name");
            ViewData["PlayerAId"] = new SelectList(_context.Players, "PlayerId", "Name");
            ViewData["PlayerBId"] = new SelectList(_context.Players, "PlayerId", "Name");

            return Page();
        }

        [BindProperty] public Game? Game { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            
            
            if (!ModelState.IsValid)
            {
                Console.WriteLine("Not Valid!");
                return Page();
            }

            _context.Games.Add(Game!);
            Console.WriteLine(Game!.GameId);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}