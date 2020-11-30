using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using GameBrain;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Game = Domain.Game;

namespace WebApp.Pages.GamePlay
{
    public class Index : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly DAL.AppDbContext _context;


        public Index(ILogger<IndexModel> logger, DAL.AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public Game? Game { get; set; }

        public Player? PLayerA { get; set; }
        public Player? PLayerB { get; set; }

        public async Task OnGetAsync(int id, int? x, int? y)
        {
            Game = await _context.Games.Where(x => x.GameId == id)
                .Include(o => o.GameOption)
                .Include(a => a.PlayerA)
                .ThenInclude(s => s.PlayerBoardStates)
                .Include(s => s.PlayerA.GameShips)
                .Include(b => b.PlayerB)
                .ThenInclude(s => s.PlayerBoardStates)
                .Include(s => s.PlayerB.GameShips)
                .FirstOrDefaultAsync();
            
            PLayerA = new Player()
            {
                Name = Game.PlayerA.Name,
                PlayerType = Game.PlayerA.PlayerType,
            };
            var playerAShips = Game!.PlayerA!.GameShips!
                .Where(s => s.PlayerId == Game.PlayerA.PlayerId).ToList();
            foreach (var ship in playerAShips)
            {
                PLayerA.Ships.Add(new Ship()
                {
                    Width = ship.Width,
                    CellState = ship.ECellState,
                    Hits = ship.Hits,
                    Name = ship.Name
                });
            }
            var playerABoardString = Game!.PlayerA!.PlayerBoardStates!
                .OrderByDescending(i => i.PlayerBoardStateId)
                .FirstOrDefault(i => i.PlayerId == Game.PlayerA.PlayerId)!.GameBoardState;
            var playerABoardState = JsonSerializer.Deserialize<GameBoardState>(playerABoardString)!.Board;
            var playerAGameBoard =  new GameBoard(Game.GameOption.BoardWidth, Game.GameOption.BoardHeight);
            for (var i = 0; i < Game.GameOption.BoardWidth; i++)
            {
                for (var j = 0; j < Game.GameOption.BoardHeight; j++)
                {
                    playerAGameBoard.Board[i, j] = playerABoardState[i][j];
                }
            }
            PLayerA.GameBoard = playerAGameBoard;

            PLayerB = new Player
            {
                Name = Game.PlayerB.Name,
                PlayerType = Game.PlayerB.PlayerType
            };
            var playerBShips = Game!.PlayerB!.GameShips!
                .Where(s => s.PlayerId == Game.PlayerB.PlayerId).ToList();
            foreach (var ship in playerBShips)
            {
                PLayerB.Ships.Add(new Ship()
                {
                    Width = ship.Width,
                    CellState = ship.ECellState,
                    Hits = ship.Hits,
                    Name = ship.Name
                });
            }
            var playerBBoardString = Game!.PlayerB!.PlayerBoardStates!
                .OrderByDescending(i => i.PlayerBoardStateId)
                .FirstOrDefault(i => i.PlayerId == Game.PlayerB.PlayerId)!.GameBoardState;
            var playerBBoardState = JsonSerializer.Deserialize<GameBoardState>(playerBBoardString)!.Board;
            var playerBGameBoard =  new GameBoard(Game.GameOption.BoardWidth, Game.GameOption.BoardHeight);
            for (var i = 0; i < Game.GameOption.BoardWidth; i++)
            {
                for (var j = 0; j < Game.GameOption.BoardHeight; j++)
                {
                    playerBGameBoard.Board[i, j] = playerBBoardState[i][j];
                }
            }
            PLayerB.GameBoard = playerBGameBoard;

            if (x != null && y != null)
            {
                PLayerA.PlaceBomb(x.Value, y.Value, PLayerB);
                Console.WriteLine(PLayerB.GameBoard.Board[0, 0]);
            }
        }

    }
}