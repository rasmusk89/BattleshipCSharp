using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using DAL;
using Domain;
using Domain.Enums;
using GameBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Razor;
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
            if (!_context.GameOptions.Any())
            {
                return RedirectToPage("/Index");
            }
            var numberOfShips = _context.GameOptions.OrderByDescending(i => i.GameOptionId).First().NumberOfShips;
            Game = await _context.Games.Where(i => i.GameId == id)
                .Include(o => o.GameOption)
                .Include(a => a.PlayerA)
                .ThenInclude(s => s.PlayerBoardStates!
                    .OrderByDescending(i => i.PlayerBoardStateId)
                    .Take(1))
                .Include(s => s.PlayerA.GameShips!
                    .OrderByDescending(i => i.GameShipId)
                    .Take(numberOfShips))
                .Include(b => b.PlayerB)
                .ThenInclude(s => s.PlayerBoardStates!
                    .OrderByDescending(i => i.PlayerBoardStateId)
                    .Take(1))
                .Include(s => s.PlayerB.GameShips!
                    .OrderByDescending(i => i.GameShipId)
                    .Take(numberOfShips))
                .FirstOrDefaultAsync();

            if (Game == null)
            {
                return RedirectToPage("/Index");
            }

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

            var playerBBoardString = Game.PlayerB.PlayerBoardStates!.Last().GameBoardState;
            var playerBBoardState = JsonSerializer.Deserialize<GameBoardState>(playerBBoardString)!.Board;
            var playerBGameBoard = new GameBoard(Game.GameOption.BoardWidth, Game.GameOption.BoardHeight);
            for (var i = 0; i < Game.GameOption.BoardWidth; i++)
            {
                for (var j = 0; j < Game.GameOption.BoardHeight; j++)
                {
                    playerBGameBoard.Board[i, j] = playerBBoardState[i][j];
                }
            }

            PLayerB.GameBoard = playerBGameBoard;

            if (newGame)
            {
                PLayerA.PlaceRandomShips();
                var boardA = new PlayerBoardState
                {
                    CreatedAt = DateTime.Now,
                    GameBoardState = PLayerA.GetSerializedGameBoardState(),
                };
                Game.PlayerA.PlayerBoardStates.Add(boardA);

                PLayerB.PlaceRandomShips();
                var boardB = new PlayerBoardState
                {
                    CreatedAt = DateTime.Now,
                    GameBoardState = PLayerB.GetSerializedGameBoardState(),
                };
                Game.PlayerB.PlayerBoardStates.Add(boardB);


                await _context.SaveChangesAsync();
                return RedirectToPage("/GamePlay/Index", new {id = Game.GameId});
            }

            if (x == null || y == null) return Page();
            {
                if (Game!.NextMoveByPlayerOne)
                {
                    var isAvailable = PLayerB.GameBoard.Board[x.Value, y.Value] != ECellState.Bomb &&
                                      PLayerB.GameBoard.Board[x.Value, y.Value] != ECellState.Hit;

                    if (!isAvailable)
                    {
                        return Page();
                    }

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
                    if (PLayerA.HasLost)
                    {
                        return RedirectToPage("/Privacy", new {name = PLayerA.Name});
                    }
                    if (PLayerB.HasLost)
                    {
                        return RedirectToPage("/Privacy", new {name = PLayerB.Name});
                    }
                    if (PLayerB.GetPlayerType() == EPlayerType.Ai)
                    {
                        var (column, row) = PLayerB.GetRandomBombCoordinates(PLayerA);
                        PLayerB.PlaceBomb(column, row, PLayerA);
                        var playerABoard = new PlayerBoardState
                        {
                            CreatedAt = DateTime.Now,
                            GameBoardState = PLayerA.GetSerializedGameBoardState(),
                        };
                        Game!.PlayerA.PlayerBoardStates!.Add(playerABoard);
                        if (PLayerA.HasLost)
                        {
                            return RedirectToPage("/Privacy", new {name = PLayerA.Name});
                        }
                        if (PLayerB.HasLost)
                        {
                            return RedirectToPage("/Privacy", new {name = PLayerB.Name});
                        }
                        Game.NextMoveByPlayerOne = true;
                        await _context.SaveChangesAsync();
                        return RedirectToPage("/GamePlay/Index", new {id = Game.GameId,}); 
                    }
                }
                else
                {
                    if (PLayerB.PlayerType != EPlayerType.Ai)
                    {
                        var isAvailable = PLayerA.GameBoard.Board[x.Value, y.Value] != ECellState.Bomb &&
                                         PLayerA.GameBoard.Board[x.Value, y.Value] != ECellState.Hit;

                        if (!isAvailable)
                        {
                            Console.WriteLine("Bomb already placed there!");
                            return Page();
                        }

                        var isHit = PLayerB.PlaceBomb(x.Value, y.Value, PLayerA);

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
                    }
                   
                    Game.NextMoveByPlayerOne = true;
                }
            }
            
            await _context.SaveChangesAsync();
            if (PLayerA.HasLost)
            {
                return RedirectToPage("/Privacy", new {name = PLayerA.Name});
            }
            if (PLayerB.HasLost)
            {
                return RedirectToPage("/Privacy", new {name = PLayerB.Name});
            }
            return Page();
        }
    }
}