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

        public Player PlayerA { get; set; } = new();
        public Player PlayerB { get; set; } = new();

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

            PlayerA.Name = Game!.PlayerA.Name;
            PlayerA.PlayerType = Game.PlayerA.PlayerType;
            PlayerA.GameBoard = new GameBoard(Game.GameOption.BoardWidth, Game.GameOption.BoardHeight);

            PlayerB.Name = Game.PlayerB.Name;
            PlayerB.PlayerType = Game.PlayerB.PlayerType;
            PlayerB.GameBoard = new GameBoard(Game.GameOption.BoardWidth, Game.GameOption.BoardHeight);

            foreach (var ship in Game.PlayerA.GameShips!)
            {
                PlayerA.Ships.Add(new Ship
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
                    PlayerA.GameBoard.Board[i, j] = playerABoardState[i][j];
                }
            }

            foreach (var ship in Game.PlayerB.GameShips!)
            {
                PlayerB.Ships.Add(new Ship
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
                    PlayerB.GameBoard.Board[i, j] = playerBBoardState[i][j];
                }
            }

            if (PlayerA.HasLost)
            {
                await _context.SaveChangesAsync();
                return RedirectToPage("/GameOver", new {name = PlayerB.Name});
            }

            if (PlayerB.HasLost)
            {
                await _context.SaveChangesAsync();
                return RedirectToPage("/GameOver", new {name = PlayerA.Name});
            }

            if (x == null || y == null) return Page();
            
// ---------------------------------------------------------------------------------------------------------------------
// ---------------PLACING BOMBS!----------------------------------------------------------------------------------------
// ---------------------------------------------------------------------------------------------------------------------
            
            if (NextMoveByPlayerA)
            {
                if (PlayerA.GetPlayerType() == EPlayerType.Ai)
                {
                    var (column, row) = PlayerA.GetRandomBombCoordinates(PlayerB);
                    var isHitAi = Player.PlaceBomb(column, row, PlayerB);

                    if (PlayerB.HasLost)
                    {
                        await _context.SaveChangesAsync();
                        return RedirectToPage("/GameOver", new {name = PlayerA.Name});
                    }

                    if (Game.GameOption.NextMoveAfterHit == ENextMoveAfterHit.SamePlayer)
                    {
                        while (isHitAi)
                        {
                            (column, row) = PlayerA.GetRandomBombCoordinates(PlayerB);
                            isHitAi = Player.PlaceBomb(column, row, PlayerB);
                            await _context.SaveChangesAsync();
                            Game.GameStates!.Add(new Domain.GameState
                            {
                                NextMoveByPlayerA = true,
                                PlayerABoardState = PlayerA.GetSerializedGameBoardState(),
                                PlayerBBoardState = PlayerB.GetSerializedGameBoardState()
                            });
                            await _context.SaveChangesAsync();
                            if (PlayerB.HasLost)
                            {
                                return RedirectToPage("/GameOver", new {name = PlayerA.Name});
                            }
                        }
                    }

                    Game.GameStates!.Add(new Domain.GameState
                    {
                        NextMoveByPlayerA = false,
                        PlayerABoardState = PlayerA.GetSerializedGameBoardState(),
                        PlayerBBoardState = PlayerB.GetSerializedGameBoardState()
                    });
                    await _context.SaveChangesAsync();
                    return PlayerB.HasLost
                        ? RedirectToPage("/GameOVer", new {name = PlayerA.Name})
                        : RedirectToPage("/GamePlay/Index", new {id = Game.GameId});
                }
                
                var isAvailable = PlayerB.GameBoard.Board[x.Value, y.Value] != ECellState.Bomb &&
                                  PlayerB.GameBoard.Board[x.Value, y.Value] != ECellState.Hit;
                if (!isAvailable)
                {
                    return Page();
                }

                var isHit = Player.PlaceBomb(x.Value, y.Value, PlayerB);

                foreach (var ship in PlayerB.Ships)
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
                            PlayerABoardState = PlayerA.GetSerializedGameBoardState(),
                            PlayerBBoardState = PlayerB.GetSerializedGameBoardState()
                        });
                        await _context.SaveChangesAsync();
                        return PlayerB.HasLost
                            ? RedirectToPage("/GameOver", new {name = PlayerA.Name})
                            : RedirectToPage("/GamePlay/Index", new {id = Game.GameId});
                    }
                }
                
                Game.GameStates!.Add(new Domain.GameState
                {
                    NextMoveByPlayerA = false,
                    PlayerABoardState = PlayerA.GetSerializedGameBoardState(),
                    PlayerBBoardState = PlayerB.GetSerializedGameBoardState()
                });
                await _context.SaveChangesAsync();
                return PlayerB.HasLost
                    ? RedirectToPage("/GameOver", new {name = PlayerA.Name})
                    : RedirectToPage("/GamePlay/Index", new {id = Game.GameId});
                
            }
            else
            {
                if (PlayerB.GetPlayerType() == EPlayerType.Ai)
                {
                    var (column, row) = PlayerB.GetRandomBombCoordinates(PlayerA);
                    var isHitAi = Player.PlaceBomb(column, row, PlayerA);
                
                    if (PlayerA.HasLost)
                    {
                        await _context.SaveChangesAsync();
                        return RedirectToPage("/GameOver", new {name = PlayerB.Name});
                    }
                
                    if (Game.GameOption.NextMoveAfterHit == ENextMoveAfterHit.SamePlayer)
                    {
                        while (isHitAi)
                        {
                            (column, row) = PlayerB.GetRandomBombCoordinates(PlayerA);
                            isHitAi = Player.PlaceBomb(column, row, PlayerA);
                            await _context.SaveChangesAsync();
                            Game.GameStates!.Add(new Domain.GameState
                            {
                                NextMoveByPlayerA = false,
                                PlayerABoardState = PlayerA.GetSerializedGameBoardState(),
                                PlayerBBoardState = PlayerB.GetSerializedGameBoardState()
                            });
                            await _context.SaveChangesAsync();
                            if (PlayerA.HasLost)
                            {
                                return RedirectToPage("/GameOver", new {name = PlayerB.Name});
                            }
                        }
                    }
                
                    Game.GameStates!.Add(new Domain.GameState
                    {
                        NextMoveByPlayerA = true,
                        PlayerABoardState = PlayerA.GetSerializedGameBoardState(),
                        PlayerBBoardState = PlayerB.GetSerializedGameBoardState()
                    });
                    await _context.SaveChangesAsync();
                    return PlayerA.HasLost
                        ? RedirectToPage("/GameOver", new {name = PlayerB.Name})
                        : RedirectToPage("/GamePlay/Index", new {id = Game.GameId});
                }
                
                var isAvailable = PlayerA.GameBoard.Board[x.Value, y.Value] != ECellState.Bomb &&
                                  PlayerA.GameBoard.Board[x.Value, y.Value] != ECellState.Hit;

                if (!isAvailable)
                {
                    Console.WriteLine("Bomb already placed there!");
                    return Page();
                }

                var isHit = Player.PlaceBomb(x.Value, y.Value, PlayerA);

                foreach (var ship in PlayerA.Ships)
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
                            PlayerABoardState = PlayerA.GetSerializedGameBoardState(),
                            PlayerBBoardState = PlayerB.GetSerializedGameBoardState()
                        });
                        await _context.SaveChangesAsync();
                        return PlayerA.HasLost
                            ? RedirectToPage("/GameOver", new {name = PlayerB.Name})
                            : RedirectToPage("/GamePlay/Index", new {id = Game.GameId});
                    }

                    Game.GameStates!.Add(new Domain.GameState
                    {
                        NextMoveByPlayerA = true,
                        PlayerABoardState = PlayerA.GetSerializedGameBoardState(),
                        PlayerBBoardState = PlayerB.GetSerializedGameBoardState()
                    });
                    await _context.SaveChangesAsync();
                    return PlayerA.HasLost
                        ? RedirectToPage("/GameOver", new {name = PlayerB.Name})
                        : RedirectToPage("/GamePlay/Index", new {id = Game.GameId});
                }

                Game.GameStates!.Add(new Domain.GameState
                {
                    NextMoveByPlayerA = true,
                    PlayerABoardState = PlayerA.GetSerializedGameBoardState(),
                    PlayerBBoardState = PlayerB.GetSerializedGameBoardState()
                });
                await _context.SaveChangesAsync();
                return PlayerA.HasLost
                    ? RedirectToPage("/GameOver", new {name = PlayerB.Name})
                    : RedirectToPage("/GamePlay/Index", new {id = Game.GameId});
            }
        }
    }
}