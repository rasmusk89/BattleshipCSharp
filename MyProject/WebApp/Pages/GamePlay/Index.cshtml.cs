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

        public bool NextMoveByPlayerA { get; set; }

        public async Task<IActionResult> OnGetAsync(int id, int? x, int? y)
        {
            if (!_context.GameOptions.Any())
            {
                return RedirectToPage("/Index");
            }

            var numberOfShips = _context.GameOptions.OrderByDescending(i => i.GameOptionId).First().NumberOfShips;
            Game = await _context.Games.Where(i => i.GameId == id)
                .Include(o => o.GameOption)
                .Include(s => s.GameStates!
                    .OrderByDescending(i => i.GameStateId)
                    .Take(1))
                .Include(a => a.PlayerA)
                .Include(s => s.PlayerA.GameShips!
                    .OrderByDescending(i => i.GameShipId)
                    .Take(numberOfShips))
                .Include(b => b.PlayerB)
                .Include(s => s.PlayerB.GameShips!
                    .OrderByDescending(i => i.GameShipId)
                    .Take(numberOfShips))
                .FirstOrDefaultAsync();

            NextMoveByPlayerA = Game.GameStates!
                .OrderByDescending(i => i.GameStateId)
                .FirstOrDefault()!.NextMoveByPlayerA;

            var lastGameState = Game.GameStates.OrderByDescending(i => i.GameStateId)
                .First();

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

            foreach (var ship in Game.PlayerA.GameShips!)
            {
                PLayerA.Ships.Add(new Ship
                {
                    Width = ship.Width,
                    CellState = ship.ECellState,
                    Hits = ship.Hits,
                    Name = ship.Name
                });
            }

            var playerABoardState = JsonSerializer
                .Deserialize<GameBoardState>(lastGameState.PlayerABoardState!)!.Board;

            for (var i = 0; i < Game.GameOption.BoardWidth; i++)
            {
                for (var j = 0; j < Game.GameOption.BoardHeight; j++)
                {
                    PLayerA.GameBoard.Board[i, j] = playerABoardState[i][j];
                }
            }

            foreach (var ship in Game.PlayerB.GameShips!)
            {
                PLayerB.Ships.Add(new Ship
                {
                    Width = ship.Width,
                    CellState = ship.ECellState,
                    Hits = ship.Hits,
                    Name = ship.Name
                });
            }

            var playerBBoardState = JsonSerializer
                .Deserialize<GameBoardState>(lastGameState.PlayerBBoardState!)!.Board;

            for (var i = 0; i < Game.GameOption.BoardWidth; i++)
            {
                for (var j = 0; j < Game.GameOption.BoardHeight; j++)
                {
                    PLayerB.GameBoard.Board[i, j] = playerBBoardState[i][j];
                }
            }

            if (PLayerA.HasLost)
            {
                await _context.SaveChangesAsync();
                return RedirectToPage("/Privacy", new {name = PLayerB.Name});
            }

            if (PLayerB.HasLost)
            {
                await _context.SaveChangesAsync();
                return RedirectToPage("/Privacy", new {name = PLayerA.Name});
            }

            if (x == null || y == null) return Page();
            
// ---------------------------------------------------------------------------------------------------------------------
// ---------------PLACING BOMBS!----------------------------------------------------------------------------------------
// ---------------------------------------------------------------------------------------------------------------------
            
            if (NextMoveByPlayerA)
            {
                if (PLayerA.GetPlayerType() == EPlayerType.Ai)
                {
                    var (column, row) = PLayerA.GetRandomBombCoordinates(PLayerB);
                    var isHitAi = PLayerA.PlaceBomb(column, row, PLayerB);

                    if (PLayerB.HasLost)
                    {
                        await _context.SaveChangesAsync();
                        return RedirectToPage("/Privacy", new {name = PLayerA.Name});
                    }

                    if (Game.GameOption.NextMoveAfterHit == ENextMoveAfterHit.SamePlayer)
                    {
                        while (isHitAi)
                        {
                            (column, row) = PLayerA.GetRandomBombCoordinates(PLayerB);
                            isHitAi = PLayerA.PlaceBomb(column, row, PLayerB);
                            await _context.SaveChangesAsync();
                            Game.GameStates!.Add(new Domain.GameState
                            {
                                NextMoveByPlayerA = true,
                                PlayerABoardState = PLayerA.GetSerializedGameBoardState(),
                                PlayerBBoardState = PLayerB.GetSerializedGameBoardState()
                            });
                            await _context.SaveChangesAsync();
                            if (PLayerB.HasLost)
                            {
                                return RedirectToPage("/Privacy", new {name = PLayerA.Name});
                            }
                        }
                    }

                    Game.GameStates!.Add(new Domain.GameState
                    {
                        NextMoveByPlayerA = false,
                        PlayerABoardState = PLayerA.GetSerializedGameBoardState(),
                        PlayerBBoardState = PLayerB.GetSerializedGameBoardState()
                    });
                    await _context.SaveChangesAsync();
                    return PLayerB.HasLost
                        ? RedirectToPage("/Privacy", new {name = PLayerA.Name})
                        : RedirectToPage("/GamePlay/Index", new {id = Game.GameId});
                }
                
                var isAvailable = PLayerB.GameBoard.Board[x.Value, y.Value] != ECellState.Bomb &&
                                  PLayerB.GameBoard.Board[x.Value, y.Value] != ECellState.Hit;
                if (!isAvailable)
                {
                    return Page();
                }

                var isHit = PLayerA.PlaceBomb(x.Value, y.Value, PLayerB);

                foreach (var ship in PLayerB.Ships)
                {
                    Game.PlayerB.GameShips!.Add(new GameShip
                    {
                        ECellState = ship.CellState,
                        Hits = ship.Hits,
                        Name = ship.Name,
                        Width = ship.Width
                    });
                }

                if (Game.GameOption.NextMoveAfterHit == ENextMoveAfterHit.SamePlayer)
                {
                    if (isHit)
                    {
                        Game.GameStates!.Add(new Domain.GameState
                        {
                            NextMoveByPlayerA = true,
                            PlayerABoardState = PLayerA.GetSerializedGameBoardState(),
                            PlayerBBoardState = PLayerB.GetSerializedGameBoardState()
                        });
                        await _context.SaveChangesAsync();
                        return PLayerB.HasLost
                            ? RedirectToPage("/Privacy", new {name = PLayerA.Name})
                            : RedirectToPage("/GamePlay/Index", new {id = Game.GameId});
                    }
                }
                
                Game.GameStates!.Add(new Domain.GameState
                {
                    NextMoveByPlayerA = false,
                    PlayerABoardState = PLayerA.GetSerializedGameBoardState(),
                    PlayerBBoardState = PLayerB.GetSerializedGameBoardState()
                });
                await _context.SaveChangesAsync();
                return PLayerB.HasLost
                    ? RedirectToPage("/Privacy", new {name = PLayerA.Name})
                    : RedirectToPage("/GamePlay/Index", new {id = Game.GameId});
                
            }
            else
            {
                if (PLayerB.GetPlayerType() == EPlayerType.Ai)
                {
                    var (column, row) = PLayerB.GetRandomBombCoordinates(PLayerA);
                    var isHitAi = PLayerB.PlaceBomb(column, row, PLayerA);
                
                    if (PLayerA.HasLost)
                    {
                        await _context.SaveChangesAsync();
                        return RedirectToPage("/Privacy", new {name = PLayerB.Name});
                    }
                
                    if (Game.GameOption.NextMoveAfterHit == ENextMoveAfterHit.SamePlayer)
                    {
                        while (isHitAi)
                        {
                            (column, row) = PLayerB.GetRandomBombCoordinates(PLayerA);
                            isHitAi = PLayerB.PlaceBomb(column, row, PLayerA);
                            await _context.SaveChangesAsync();
                            Game.GameStates!.Add(new Domain.GameState
                            {
                                NextMoveByPlayerA = false,
                                PlayerABoardState = PLayerA.GetSerializedGameBoardState(),
                                PlayerBBoardState = PLayerB.GetSerializedGameBoardState()
                            });
                            await _context.SaveChangesAsync();
                            if (PLayerA.HasLost)
                            {
                                return RedirectToPage("/Privacy", new {name = PLayerB.Name});
                            }
                        }
                    }
                
                    Game.GameStates!.Add(new Domain.GameState
                    {
                        NextMoveByPlayerA = true,
                        PlayerABoardState = PLayerA.GetSerializedGameBoardState(),
                        PlayerBBoardState = PLayerB.GetSerializedGameBoardState()
                    });
                    await _context.SaveChangesAsync();
                    return PLayerA.HasLost
                        ? RedirectToPage("/Privacy", new {name = PLayerB.Name})
                        : RedirectToPage("/GamePlay/Index", new {id = Game.GameId});
                }
                
                var isAvailable = PLayerA.GameBoard.Board[x.Value, y.Value] != ECellState.Bomb &&
                                  PLayerA.GameBoard.Board[x.Value, y.Value] != ECellState.Hit;

                if (!isAvailable)
                {
                    Console.WriteLine("Bomb already placed there!");
                    return Page();
                }

                var isHit = PLayerB.PlaceBomb(x.Value, y.Value, PLayerA);

                foreach (var ship in PLayerA.Ships)
                {
                    Game.PlayerA.GameShips!.Add(new GameShip
                    {
                        ECellState = ship.CellState,
                        Hits = ship.Hits,
                        Name = ship.Name,
                        Width = ship.Width
                    });
                }

                if (Game.GameOption.NextMoveAfterHit == ENextMoveAfterHit.SamePlayer)
                {
                    if (isHit)
                    {
                        Game.GameStates!.Add(new Domain.GameState
                        {
                            NextMoveByPlayerA = false,
                            PlayerABoardState = PLayerA.GetSerializedGameBoardState(),
                            PlayerBBoardState = PLayerB.GetSerializedGameBoardState()
                        });
                        await _context.SaveChangesAsync();
                        return PLayerA.HasLost
                            ? RedirectToPage("/Privacy", new {name = PLayerB.Name})
                            : RedirectToPage("/GamePlay/Index", new {id = Game.GameId});
                    }

                    Game.GameStates!.Add(new Domain.GameState
                    {
                        NextMoveByPlayerA = true,
                        PlayerABoardState = PLayerA.GetSerializedGameBoardState(),
                        PlayerBBoardState = PLayerB.GetSerializedGameBoardState()
                    });
                    await _context.SaveChangesAsync();
                    return PLayerA.HasLost
                        ? RedirectToPage("/Privacy", new {name = PLayerB.Name})
                        : RedirectToPage("/GamePlay/Index", new {id = Game.GameId});
                }

                Game.GameStates!.Add(new Domain.GameState
                {
                    NextMoveByPlayerA = true,
                    PlayerABoardState = PLayerA.GetSerializedGameBoardState(),
                    PlayerBBoardState = PLayerB.GetSerializedGameBoardState()
                });
                await _context.SaveChangesAsync();
                return PLayerA.HasLost
                    ? RedirectToPage("/Privacy", new {name = PLayerB.Name})
                    : RedirectToPage("/GamePlay/Index", new {id = Game.GameId});
            }
        }
    }
}