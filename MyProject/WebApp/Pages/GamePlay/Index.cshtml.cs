using System.Collections.Generic;
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
using GameState = Domain.GameState;
using Player = GameBrain.Player;

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

        public string? Message { get; set; } = "";

        public bool NextMoveByPlayerA { get; set; }

        public async Task<IActionResult> OnGetAsync(int id, int? x, int? y, string? message)
        {
            const string? hitMessage = "HIT!";
            const string? moveAgainMessage = " Move again!";
            const string? missMessage = "MISS";
            const string? errorMessage = "Bomb already placed there";

            if (message != null)
            {
                Message = message;
            }

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
                PlayerA.Ships.Add(new Ship(ship.Width)
                {
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
                PlayerB.Ships.Add(new Ship(ship.Width)
                {
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
            
            // Player A move.
            if (NextMoveByPlayerA)
            {
                if (PlayerA.GetPlayerType() == EPlayerType.Human)
                {
                    if (!BombAreaAvailable(PlayerB, x.Value, y.Value))
                    {
                        return RedirectToPage("/GamePlay/Index", new {id = Game.GameId, message = errorMessage});
                    }

                    var isHit = PlayerA.PlaceBomb(x.Value, y.Value, PlayerB);

                    AddOpponentShipsToDb(PlayerB, Game.PlayerB.GameShips);

                    if (Game.GameOption.NextMoveAfterHit == ENextMoveAfterHit.SamePlayer)
                    {
                        if (isHit)
                        {
                            AddGameStateToDb(Game, true);

                            await _context.SaveChangesAsync();
                            return PlayerB.HasLost
                                ? RedirectToPage("/GameOver", new {name = PlayerA.Name})
                                : RedirectToPage("/GamePlay/Index",
                                    new {id = Game.GameId, message = hitMessage + moveAgainMessage});
                        }
                    }

                    AddGameStateToDb(Game, false);
                    await _context.SaveChangesAsync();

                    if (PlayerB.GetPlayerType() == EPlayerType.Ai)
                    {
                        var (column, row) = PlayerB.GetRandomBombCoordinates(PlayerA);
                        var isHitAi = PlayerB.PlaceBomb(column, row, PlayerA);

                        AddOpponentShipsToDb(PlayerA, Game.PlayerA.GameShips);

                        if (Game.GameOption.NextMoveAfterHit == ENextMoveAfterHit.SamePlayer)
                        {
                            while (isHitAi)
                            {
                                (column, row) = PlayerB.GetRandomBombCoordinates(PlayerA);
                                isHitAi = PlayerB.PlaceBomb(column, row, PlayerA);

                                AddOpponentShipsToDb(PlayerA, Game.PlayerA.GameShips);
                                AddGameStateToDb(Game, false);

                                await _context.SaveChangesAsync();
                                if (PlayerA.HasLost)
                                {
                                    return RedirectToPage("/GameOver", new {name = PlayerB.Name});
                                }
                            }
                        }

                        AddGameStateToDb(Game, true);

                        await _context.SaveChangesAsync();
                        return PlayerA.HasLost
                            ? RedirectToPage("/GameOVer", new {name = PlayerB.Name})
                            : RedirectToPage("/GamePlay/Index", new {id = Game.GameId});
                    }

                    return PlayerB.HasLost
                        ? RedirectToPage("/GameOver", new {name = PlayerA.Name})
                        : RedirectToPage("/Confirmation/Index",
                            new
                            {
                                id = Game.GameId,
                                message = isHit ? hitMessage : missMessage,
                                player = PlayerA.Name,
                                opponent = PlayerB.Name
                            });
                }

                if (PlayerA.GetPlayerType() == EPlayerType.Ai)
                {
                    var (column, row) = PlayerA.GetRandomBombCoordinates(PlayerB);
                    var isHitAi = PlayerA.PlaceBomb(column, row, PlayerB);

                    AddOpponentShipsToDb(PlayerB, Game.PlayerB.GameShips);

                    if (Game.GameOption.NextMoveAfterHit == ENextMoveAfterHit.SamePlayer)
                    {
                        while (isHitAi)
                        {
                            (column, row) = PlayerA.GetRandomBombCoordinates(PlayerB);
                            isHitAi = PlayerA.PlaceBomb(column, row, PlayerB);

                            AddOpponentShipsToDb(PlayerB, Game.PlayerB.GameShips);
                            AddGameStateToDb(Game, true);

                            await _context.SaveChangesAsync();
                            if (PlayerB.HasLost)
                            {
                                return RedirectToPage("/GameOver", new {name = PlayerA.Name});
                            }
                        }
                    }

                    AddGameStateToDb(Game, false);
                    await _context.SaveChangesAsync();

                    return PlayerB.HasLost
                        ? RedirectToPage("/GameOVer", new {name = PlayerA.Name})
                        : RedirectToPage("/GamePlay/Index",
                            new {id = Game.GameId, message = isHitAi ? hitMessage : missMessage});
                }
            }
            
            // Player B move!
            else
            {
                if (PlayerB.GetPlayerType() == EPlayerType.Human)
                {
                    if (!BombAreaAvailable(PlayerA, x.Value, y.Value))
                    {
                        return RedirectToPage("/GamePlay/Index", new {id = Game.GameId, message = errorMessage});
                    }

                    var isHit = PlayerB.PlaceBomb(x.Value, y.Value, PlayerA);

                    AddOpponentShipsToDb(PlayerA, Game.PlayerA.GameShips);

                    if (Game.GameOption.NextMoveAfterHit == ENextMoveAfterHit.SamePlayer)
                    {
                        if (isHit)
                        {
                            AddGameStateToDb(Game, false);

                            await _context.SaveChangesAsync();
                            return PlayerA.HasLost
                                ? RedirectToPage("/GameOver", new {name = PlayerB.Name})
                                : RedirectToPage("/GamePlay/Index",
                                    new {id = Game.GameId, message = hitMessage + moveAgainMessage});
                        }
                    }

                    AddGameStateToDb(Game, true);
                    await _context.SaveChangesAsync();

                    if (PlayerA.GetPlayerType() == EPlayerType.Ai)
                    {
                        var (column, row) = PlayerA.GetRandomBombCoordinates(PlayerB);
                        var isHitAi = PlayerA.PlaceBomb(column, row, PlayerB);

                        AddOpponentShipsToDb(PlayerB, Game.PlayerB.GameShips);

                        if (Game.GameOption.NextMoveAfterHit == ENextMoveAfterHit.SamePlayer)
                        {
                            while (isHitAi)
                            {
                                (column, row) = PlayerA.GetRandomBombCoordinates(PlayerB);
                                isHitAi = PlayerA.PlaceBomb(column, row, PlayerB);

                                AddOpponentShipsToDb(PlayerB, Game.PlayerB.GameShips);
                                AddGameStateToDb(Game, true);

                                await _context.SaveChangesAsync();
                                if (PlayerB.HasLost)
                                {
                                    return RedirectToPage("/GameOver", new {name = PlayerA.Name});
                                }
                            }
                        }

                        AddGameStateToDb(Game, false);

                        await _context.SaveChangesAsync();
                        return PlayerB.HasLost
                            ? RedirectToPage("/GameOVer", new {name = PlayerA.Name})
                            : RedirectToPage("/GamePlay/Index", new {id = Game.GameId});
                    }

                    return PlayerA.HasLost
                        ? RedirectToPage("/GameOver", new {name = PlayerB.Name})
                        : RedirectToPage("/Confirmation/Index",
                            new
                            {
                                id = Game.GameId,
                                message = isHit ? hitMessage : missMessage,
                                player = PlayerB.Name,
                                opponent = PlayerA.Name
                            });
                }

                if (PlayerB.GetPlayerType() == EPlayerType.Ai)
                {
                    var (column, row) = PlayerB.GetRandomBombCoordinates(PlayerA);
                    var isHitAi = PlayerB.PlaceBomb(column, row, PlayerA);

                    AddOpponentShipsToDb(PlayerA, Game.PlayerA.GameShips);

                    if (Game.GameOption.NextMoveAfterHit == ENextMoveAfterHit.SamePlayer)
                    {
                        while (isHitAi)
                        {
                            (column, row) = PlayerB.GetRandomBombCoordinates(PlayerA);
                            isHitAi = PlayerB.PlaceBomb(column, row, PlayerA);

                            AddOpponentShipsToDb(PlayerA, Game.PlayerA.GameShips);
                            AddGameStateToDb(Game, false);

                            await _context.SaveChangesAsync();
                            if (PlayerA.HasLost)
                            {
                                return RedirectToPage("/GameOver", new {name = PlayerB.Name});
                            }
                        }
                    }

                    AddGameStateToDb(Game, true);

                    await _context.SaveChangesAsync();
                    return PlayerA.HasLost
                        ? RedirectToPage("/GameOVer", new {name = PlayerB.Name})
                        : RedirectToPage("/GamePlay/Index",
                            new {id = Game.GameId, message = isHitAi ? hitMessage : missMessage});
                }
            }
            return Page();
        }

        private static bool BombAreaAvailable(Player opponent, int x, int y)
        {
            var isAvailable = opponent.GameBoard.Board[x, y] != ECellState.Bomb &&
                              opponent.GameBoard.Board[x, y] != ECellState.Hit;
            return isAvailable;
        }

        private void AddGameStateToDb(Game game, bool nextMoveByPlayerA)
        {
            game.GameStates!.Add(new GameState
            {
                NextMoveByPlayerA = nextMoveByPlayerA,
                PlayerABoardState = PlayerA.GetSerializedGameBoardState(),
                PlayerBBoardState = PlayerB.GetSerializedGameBoardState()
            });
        }

        private static void AddOpponentShipsToDb(Player opponent, ICollection<GameShip> opponentGameShips)
        {
            foreach (var ship in opponent.Ships)
            {
                opponentGameShips.Add(new GameShip
                {
                    ECellState = ship.CellState,
                    Hits = ship.Hits,
                    Name = ship.Name,
                    Width = ship.Width,
                    IsSunk = ship.IsSunk
                });
            }
        }
    }
}