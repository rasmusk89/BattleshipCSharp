﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using DAL;
using Domain;
using Domain.Enums;
using GameBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Game = Domain.Game;
using GameState = Domain.GameState;
using Player = Domain.Player;

namespace WebApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly AppDbContext _context;


        public IndexModel(ILogger<IndexModel> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [BindProperty] public Player? PlayerA { get; set; }

        [BindProperty] public Player? PlayerB { get; set; }

        [BindProperty] public GameOption? GameOption { get; set; }

        public List<Game>? Games { get; set; }

        public int? Id { get; set; }

        [BindProperty]
        [Display(Name = "Random Ships")]
        public ERandomShips? RandomShips { get; set; }

        public IActionResult OnGet()
        {
            Games = _context.Games.OrderByDescending(id => id.GameId).Take(3).ToList();
            return Page();
        }

        public RedirectToPageResult OnPostLoadGame()
        {
            var gameId = int.Parse(Request.Form["GameId"]);
            return RedirectToPage("./GamePlay/Index", new {id = gameId});
        }

        public async Task<IActionResult> OnPostNewGame()
        {
            var ships = new List<Ship>();

            var numberOfShips = GameOption!.BoardWidth < GameOption.BoardHeight
                ? GameOption.BoardWidth / 2
                : GameOption.BoardHeight / 2;

            for (var i = 1; i <= numberOfShips; i++)
            {
                ships.Add(new Ship(i));
            }

            var playerA = new Player
            {
                Name = PlayerA!.Name,
                PlayerType = PlayerA!.PlayerType,
                GameShips = new List<GameShip>(),
            };

            foreach (var ship in ships)
            {
                playerA.GameShips.Add(new GameShip
                {
                    Name = ship.Name,
                    Width = ship.Width,
                    ECellState = ship.CellState
                });
            }

            var playerB = new Player
            {
                Name = PlayerB!.Name,
                PlayerType = PlayerB!.PlayerType,
                GameShips = new List<GameShip>(),
            };

            foreach (var ship in ships)
            {
                playerB.GameShips.Add(new GameShip
                {
                    Name = ship.Name,
                    Width = ship.Width,
                    ECellState = ship.CellState
                });
            }

            var game = new Game
            {
                Description = $"{PlayerA!.Name}&{PlayerB!.Name}@{DateTime.Now}".Replace(" ", ""),
                GameOption = new GameOption
                {
                    Name = DateTime.Now.ToString(CultureInfo.InvariantCulture),
                    BoardWidth = GameOption.BoardWidth,
                    BoardHeight = GameOption.BoardHeight,
                    EShipsCanTouch = GameOption.EShipsCanTouch,
                    NextMoveAfterHit = GameOption.NextMoveAfterHit,
                    NumberOfShips = ships.Count
                },
                PlayerA = playerA,
                PlayerB = playerB,
                GameStates = new List<GameState>()
            };

            game.GameStates.Add(new GameState
            {
                NextMoveByPlayerA = true,
                PlayerABoardState = GetEmptyBoard(),
                PlayerBBoardState = GetEmptyBoard()
            });

            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _context.Games.AddAsync(game);
            await _context.SaveChangesAsync();

            return RedirectToPage("/PlaceShips/Index",
                new {id = game.GameId, random = RandomShips, orientation = EOrientation.Horizontal});
        }

        private string GetEmptyBoard()
        {
            var state = new GameBoardState();
            var width = GameOption!.BoardWidth;
            var height = GameOption.BoardHeight;
            state.Board = new ECellState[width][];
            for (var i = 0; i < state.Board.Length; i++)
            {
                state.Board[i] = new ECellState[height];
            }

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    state.Board[x][y] = ECellState.Empty;
                }
            }

            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            return JsonSerializer.Serialize(state, jsonOptions);
        }
    }
}