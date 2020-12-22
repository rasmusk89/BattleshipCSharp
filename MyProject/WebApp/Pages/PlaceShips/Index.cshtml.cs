using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using DAL;
using Domain.Enums;
using GameBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Game = Domain.Game;
using GameState = Domain.GameState;
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

        public Game? Game { get; set; }

        public Player PLayerA { get; set; } = new();
        public Player PLayerB { get; set; } = new();

        public bool NextMoveByPlayerA { get; set; }

        public EOrientation Orientation { get; set; }

        public string Message { get; set; } = "";

        public async Task<IActionResult> OnGetAsync(int id, int? x, int? y, ERandomShips? random, string? message,
            EOrientation orientation)
        {
            Orientation = orientation;

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
                .ThenInclude(s => s.GameShips!
                    .OrderByDescending(i => i.GameShipId)
                    .Take(numberOfShips))
                .Include(b => b.PlayerB)
                .ThenInclude(s => s.GameShips!
                    .OrderByDescending(i => i.GameShipId)
                    .Take(numberOfShips))
                .FirstOrDefaultAsync();

            if (Game == null)
            {
                return RedirectToPage("/Index");
            }

            NextMoveByPlayerA = Game.GameStates!
                .OrderByDescending(i => i.GameStateId)
                .FirstOrDefault()!.NextMoveByPlayerA;
            var lastGameState = Game.GameStates!.OrderByDescending(i => i.GameStateId)
                .First();
            var shipsCanTouch = Game.GameOption.EShipsCanTouch;

            PLayerA.Name = Game.PlayerA.Name;
            PLayerA.PlayerType = Game.PlayerA.PlayerType;
            PLayerA.GameBoard = new GameBoard(Game.GameOption.BoardWidth, Game.GameOption.BoardHeight);
            var playerABoardState = JsonSerializer
                .Deserialize<GameBoardState>(lastGameState.PlayerABoardState!)!.Board;
            for (var i = 0; i < Game.GameOption.BoardWidth; i++)
            {
                for (var j = 0; j < Game.GameOption.BoardHeight; j++)
                {
                    PLayerA.GameBoard.Board[i, j] = playerABoardState[i][j];
                }
            }

            PLayerB.Name = Game.PlayerB.Name;
            PLayerB.PlayerType = Game.PlayerB.PlayerType;
            PLayerB.GameBoard = new GameBoard(Game.GameOption.BoardWidth, Game.GameOption.BoardHeight);
            var playerBBoardState = JsonSerializer
                .Deserialize<GameBoardState>(lastGameState.PlayerBBoardState!)!.Board;

            for (var i = 0; i < Game.GameOption.BoardWidth; i++)
            {
                for (var j = 0; j < Game.GameOption.BoardHeight; j++)
                {
                    PLayerB.GameBoard.Board[i, j] = playerBBoardState[i][j];
                }
            }

            var playerAShips = Game.PlayerA.GameShips!;

            foreach (var gameShip in playerAShips)
            {
                PLayerA.Ships.Add(new Ship(gameShip.Width)
                {
                    CellState = gameShip.ECellState,
                    Hits = gameShip.Hits,
                    Name = gameShip.Name
                });
            }

            var playerBShips = Game.PlayerB.GameShips!;

            foreach (var gameShip in playerBShips)
            {
                PLayerB.Ships.Add(new Ship(gameShip.Width)
                {
                    CellState = gameShip.ECellState,
                    Hits = gameShip.Hits,
                    Name = gameShip.Name
                });
            }

            // Placing random ships

            if (random == ERandomShips.Yes)
            {
                PLayerA.PlaceRandomShips(shipsCanTouch);
                PLayerB.PlaceRandomShips(shipsCanTouch);

                if (PLayerA.GetPlayerType() == EPlayerType.Ai && PLayerB.GetPlayerType() == EPlayerType.Human)
                {
                    Game.GameStates!.Add(new GameState
                    {
                        NextMoveByPlayerA = false,
                        PlayerABoardState = PLayerA.GetSerializedGameBoardState(),
                        PlayerBBoardState = PLayerB.GetSerializedGameBoardState()
                    });
                    await _context.SaveChangesAsync();
                    return RedirectToPage("/GamePlay/Index", new {id = Game.GameId});
                }

                Game.GameStates!.Add(new GameState
                {
                    NextMoveByPlayerA = true,
                    PlayerABoardState = PLayerA.GetSerializedGameBoardState(),
                    PlayerBBoardState = PLayerB.GetSerializedGameBoardState()
                });
                await _context.SaveChangesAsync();
                return RedirectToPage("/GamePlay/Index", new {id = Game.GameId});
            }

            // Player placing ships

            if (x == null || y == null) return Page();

            if (NextMoveByPlayerA)
            {
                if (PLayerA.GetPlayerType() == EPlayerType.Ai)
                {
                    PLayerA.PlaceRandomShips(shipsCanTouch);
                    Game.GameStates!.Add(new GameState
                    {
                        NextMoveByPlayerA = false,
                        PlayerABoardState = PLayerA.GetSerializedGameBoardState(),
                        PlayerBBoardState = PLayerB.GetSerializedGameBoardState()
                    });
                    await _context.SaveChangesAsync();
                    return RedirectToPage("/PlaceShips/Index", new {id = Game.GameId, orientation = Orientation});
                }

                foreach (var ship in PLayerA.Ships.Where(ship => !ShipPlaced(PLayerA, ship)))
                {
                    if (!PLayerA.PlaceShip(x.Value, y.Value, ship, Orientation, shipsCanTouch))
                    {
                        const string? cantPlace = "Can't place ship there";
                        return RedirectToPage("/PlaceShips/Index", new {id = Game.GameId, message = cantPlace});
                    }

                    Game.GameStates!.Add(new GameState
                    {
                        NextMoveByPlayerA = true,
                        PlayerABoardState = PLayerA.GetSerializedGameBoardState(),
                        PlayerBBoardState = PLayerB.GetSerializedGameBoardState()
                    });
                    await _context.SaveChangesAsync();
                    return RedirectToPage("/PlaceShips/Index", new {id = Game.GameId, orientation = Orientation});
                }

                Game.GameStates!.Add(new GameState
                {
                    NextMoveByPlayerA = false,
                    PlayerABoardState = PLayerA.GetSerializedGameBoardState(),
                    PlayerBBoardState = PLayerB.GetSerializedGameBoardState()
                });
                await _context.SaveChangesAsync();
                return RedirectToPage("/PlaceShips/Index", new {id = Game.GameId, orientation = Orientation});
            }

            if (PLayerB.GetPlayerType() == EPlayerType.Ai)
            {
                PLayerB.PlaceRandomShips(shipsCanTouch);
                Game.GameStates!.Add(new GameState
                {
                    NextMoveByPlayerA = true,
                    PlayerABoardState = PLayerA.GetSerializedGameBoardState(),
                    PlayerBBoardState = PLayerB.GetSerializedGameBoardState()
                });
                await _context.SaveChangesAsync();
                return RedirectToPage("/GamePlay/Index", new {id = Game.GameId});
            }

            foreach (var ship in PLayerB.Ships.Where(ship => !ShipPlaced(PLayerB, ship)))
            {
                if (!PLayerB.PlaceShip(x.Value, y.Value, ship, Orientation, shipsCanTouch))
                {
                    const string? cantPlace = "Can't place ship there";
                    return RedirectToPage("/PlaceShips/Index", new {id = Game.GameId, message = cantPlace});
                }

                Game.GameStates!.Add(new GameState
                {
                    NextMoveByPlayerA = false,
                    PlayerABoardState = PLayerA.GetSerializedGameBoardState(),
                    PlayerBBoardState = PLayerB.GetSerializedGameBoardState()
                });
                await _context.SaveChangesAsync();
                return RedirectToPage("/PlaceShips/Index", new {id = Game.GameId, orientation = Orientation});
            }


            // Do not show AI board.
            if (PLayerA.GetPlayerType() == EPlayerType.Ai && PLayerB.GetPlayerType() == EPlayerType.Human)
            {
                Game.GameStates!.Add(new GameState
                {
                    NextMoveByPlayerA = false,
                    PlayerABoardState = PLayerA.GetSerializedGameBoardState(),
                    PlayerBBoardState = PLayerB.GetSerializedGameBoardState()
                });
                await _context.SaveChangesAsync();
                return RedirectToPage("/GamePlay/Index", new {id = Game.GameId});
            }

            Game.GameStates!.Add(new GameState
            {
                NextMoveByPlayerA = true,
                PlayerABoardState = PLayerA.GetSerializedGameBoardState(),
                PlayerBBoardState = PLayerB.GetSerializedGameBoardState()
            });
            await _context.SaveChangesAsync();
            return RedirectToPage("/GamePlay/Index", new {id = Game.GameId});
        }

        private static bool ShipPlaced(Player player, Ship ship)
        {
            var width = player.GameBoard.Board.GetUpperBound(0) + 1;
            var height = player.GameBoard.Board.GetUpperBound(1) + 1;
            var shipState = ship.CellState;

            for (var i = 0; i < width; i++)
            {
                for (var j = 0; j < height; j++)
                {
                    if (player.GameBoard.Board[i, j] == shipState)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}