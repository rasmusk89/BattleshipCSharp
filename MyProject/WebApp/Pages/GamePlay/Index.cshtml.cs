using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using DAL;
using Domain;
using GameBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Game = Domain.Game;
using GameBoard = GameBrain.GameBoard;
using Player = GameBrain.Player;
using Ship = GameBrain.Ship;

namespace WebApp.Pages.GamePlay
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

        public Game? Game { get; set; }

        public Player PLayerA { get; set; } = new();
        public Player PLayerB { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id, int? x, int? y, bool newGame)
        {
// ------------------------------------------------------------------------------------------------------------------ // 
            var watch = System.Diagnostics.Stopwatch.StartNew();
// ------------------------------------------------------------------------------------------------------------------ // 

            Game = await _context.Games.Where(i => i.GameId == id)
                .Include(o => o.GameOption)
                .Include(a => a.PlayerA)
                .ThenInclude(s => s.PlayerBoardStates)
                .Include(s => s.PlayerA.GameShips)
                .Include(b => b.PlayerB)
                .ThenInclude(s => s.PlayerBoardStates)
                .Include(s => s.PlayerB.GameShips)
                .FirstOrDefaultAsync();


            if (Game == null)
            {
                return RedirectToPage("/Index");
            }

// ------------------------------------------------------------------------------------------------------------------ // 
            watch.Stop();
            var time = watch.ElapsedMilliseconds;
            Console.WriteLine("Connecting to DataBase: " + Convert.ToDouble(time) / 1000 + "sec");

// ------------------------------------------------------------------------------------------------------------------ // 

            PLayerA.Name = Game!.PlayerA.Name;
            PLayerA.PlayerType = Game.PlayerA.PlayerType;
            PLayerA.GameBoard = new GameBoard(Game.GameOption.BoardWidth, Game.GameOption.BoardHeight);

            PLayerB.Name = Game.PlayerB.Name;
            PLayerB.PlayerType = Game.PlayerB.PlayerType;
            PLayerB.GameBoard = new GameBoard(Game.GameOption.BoardWidth, Game.GameOption.BoardHeight);

            var playerAShips = Game!.PlayerA!.GameShips!
                .Where(s => s.PlayerId == Game.PlayerA.PlayerId).ToList();
            foreach (var ship in playerAShips)
            {
                PLayerA.Ships.Add(new Ship
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
            var playerAGameBoard = new GameBoard(Game.GameOption.BoardWidth, Game.GameOption.BoardHeight);

            for (var i = 0; i < Game.GameOption.BoardWidth; i++)
            {
                for (var j = 0; j < Game.GameOption.BoardHeight; j++)
                {
                    playerAGameBoard.Board[i, j] = playerABoardState[i][j];
                }
            }

            PLayerA.GameBoard = playerAGameBoard;

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


            var board = Game.PlayerB.PlayerBoardStates!.Last().GameBoardState;

            var playerBBoardState = JsonSerializer.Deserialize<GameBoardState>(board)!.Board;
            var playerBGameBoard = new GameBoard(Game.GameOption.BoardWidth, Game.GameOption.BoardHeight);
            for (var i = 0; i < Game.GameOption.BoardWidth; i++)
            {
                for (var j = 0; j < Game.GameOption.BoardHeight; j++)
                {
                    playerBGameBoard.Board[i, j] = playerBBoardState[i][j];
                }
            }

            PLayerB.GameBoard = playerBGameBoard;

// ------------------------------------------------------------------------------------------------------------------ // 

// ------------------------------------------------------------------------------------------------------------------ // 

            if (newGame)
            {
                PLayerA.PlaceRandomShips();
                var boardA = new PlayerBoardState
                {
                    CreatedAt = DateTime.Now,
                    GameBoardState = PLayerA.GetSerializedGameBoardState(),
                };
                PLayerB.PlaceRandomShips();
                var boardB = new PlayerBoardState
                {
                    CreatedAt = DateTime.Now,
                    GameBoardState = PLayerB.GetSerializedGameBoardState(),
                };

                Game.PlayerA.PlayerBoardStates.Add(boardA);
                // _context.Games!
                // .First(i => i.GameId == id)
                // .PlayerA!.PlayerBoardStates!.Add(boardA);

                Game.PlayerB.PlayerBoardStates.Add(boardB);
                // _context.Games!
                // .First(i => i.GameId == id)
                // .PlayerB!.PlayerBoardStates!.Add(boardB);
            }
// ------------------------------------------------------------------------------------------------------------------ // 

// ------------------------------------------------------------------------------------------------------------------ // 

            if (x == null || y == null) return Page();
            {
                if (Game!.NextMoveByPlayerOne)
                {
                    PLayerA.PlaceBomb(x.Value, y.Value, PLayerB);

                    var playerBoardState = new PlayerBoardState
                    {
                        CreatedAt = DateTime.Now,
                        GameBoardState = PLayerB.GetSerializedGameBoardState(),
                    };

                    Game!.PlayerB.PlayerBoardStates!.Add(playerBoardState);

                    foreach (var ship in PLayerB.Ships)
                    {
                        Game.PlayerB.GameShips.Add(new GameShip()
                        {
                            ECellState = ship.CellState,
                            Hits = ship.Hits,
                            Name = ship.Name,
                            Width = ship.Width
                        });
                    }

                    Game.NextMoveByPlayerOne = false;
                    await _context.SaveChangesAsync();

// ------------------------------------------------------------------------------------------------------------------ // 

// ------------------------------------------------------------------------------------------------------------------ // 
                }
                else
                {
                    PLayerB.PlaceBomb(x.Value, y.Value, PLayerA);

                    var playerBoardState = new PlayerBoardState
                    {
                        CreatedAt = DateTime.Now,
                        GameBoardState = PLayerA.GetSerializedGameBoardState(),
                    };

                    Game.PlayerA.PlayerBoardStates.Add(playerBoardState);

                    foreach (var ship in PLayerA.Ships)
                    {
                        Game.PlayerA.GameShips.Add(new GameShip()
                        {
                            ECellState = ship.CellState,
                            Hits = ship.Hits,
                            Name = ship.Name,
                            Width = ship.Width
                        });
                    }

                    Game.NextMoveByPlayerOne = true;
                    await _context.SaveChangesAsync();

// ------------------------------------------------------------------------------------------------------------------ // 

// ------------------------------------------------------------------------------------------------------------------ // 
                }
            }

// >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> // 

// <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< // 

            return Page();
        }
    }
}