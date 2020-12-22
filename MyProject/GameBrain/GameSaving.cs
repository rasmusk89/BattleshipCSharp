using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DAL;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace GameBrain
{
    public static class GameSaving
    {
        // Save when starting game.
        public static void InitialSave(GameState gameState)
        {
            Console.Clear();
            Console.Write("Saving...");
            using var dbCtx = GetConnection();
            dbCtx.Database.Migrate();

            var playerOne = gameState.PlayerAState;
            var playerTwo = gameState.PlayerBState;

            var playerA = new Domain.Player
            {
                Name = playerOne.GetName(),
                PlayerType = playerOne.GetPlayerType(),
                GameShips = new List<GameShip>(),
            };
            var playerB = new Domain.Player
            {
                Name = playerTwo.GetName(),
                PlayerType = playerTwo.GetPlayerType(),
                GameShips = new List<GameShip>(),
            };

            var playerAShips = playerOne.GetShips()
                .Select(ship => new GameShip()
                {
                    ECellState = ship.CellState,
                    Hits = ship.Hits,
                    IsSunk = ship.IsSunk,
                    Width = ship.Width,
                    Name = ship.Name,
                    Player = playerA
                })
                .ToList();

            var playerBShips = playerTwo.GetShips()
                .Select(ship => new GameShip()
                {
                    ECellState = ship.CellState,
                    Hits = ship.Hits,
                    IsSunk = ship.IsSunk,
                    Width = ship.Width,
                    Name = ship.Name,
                    Player = playerB
                })
                .ToList();

            playerA.GameShips = playerAShips;
            playerB.GameShips = playerBShips;

            var gameOptions = new GameOption
            {
                Name = DateTime.Now.ToString(CultureInfo.CurrentCulture),
                BoardWidth = gameState.BoardWidthState,
                BoardHeight = gameState.BoardHeightState,
                EShipsCanTouch = gameState.ShipsCanTouchState,
                NextMoveAfterHit = gameState.GameOptions.GetNextMoveAfterHit(),
                NumberOfShips = playerAShips.Count
            };

            var newGame = new Domain.Game
            {
                Description = $"{playerA.Name}&{playerB.Name}@{DateTime.Now}".Replace(" ", "_"),
                GameOption = gameOptions,
                PlayerA = playerA,
                PlayerB = playerB,
                GameStates = new List<Domain.GameState>()
            };

            var state = new Domain.GameState
            {
                PlayerABoardState = playerOne.GetSerializedGameBoardState(),
                PlayerBBoardState = playerTwo.GetSerializedGameBoardState(),
                NextMoveByPlayerA = true
            };

            newGame.GameStates.Add(state);
            dbCtx.Games.Add(newGame);
            dbCtx.SaveChanges();
        }

        // Save when running game
        public static void SaveGameState(GameState gameState)
        {
            using var dbCtx = GetConnection();

            var numberOfShips = dbCtx.GameOptions.OrderByDescending(i => i.GameOptionId).First().NumberOfShips;
            var lastGame = dbCtx.Games
                .OrderByDescending(id => id.GameId)
                .Include(s => s.GameStates!
                    .OrderByDescending(i => i.GameStateId)
                    .Take(1))
                .Include(p => p.PlayerA)
                .Include(s => s.PlayerA.GameShips!
                    .OrderByDescending(x => x.GameShipId)
                    .Take(numberOfShips))
                .Include(p => p.PlayerB)
                .Include(s => s.PlayerB.GameShips!
                    .OrderByDescending(x => x.GameShipId)
                    .Take(numberOfShips))
                .First();

            foreach (var ship in gameState.PlayerAState.GetShips())
            {
                dbCtx.GameShips.Add(new GameShip
                {
                    ECellState = ship.CellState,
                    Hits = ship.Hits,
                    Name = ship.Name,
                    Width = ship.Width,
                    Player = lastGame.PlayerA,
                    IsSunk = ship.IsSunk
                });
            }

            foreach (var ship in gameState.PlayerBState.GetShips())
            {
                lastGame.PlayerB.GameShips!.Add(new GameShip
                {
                    ECellState = ship.CellState,
                    Hits = ship.Hits,
                    Name = ship.Name,
                    Width = ship.Width,
                    Player = lastGame.PlayerB,
                    IsSunk = ship.IsSunk
                });
            }

            lastGame.GameStates!.Add(new Domain.GameState()
            {
                NextMoveByPlayerA = gameState.NextMoveByPlayerAState,
                PlayerABoardState = gameState.PlayerAState.GetSerializedGameBoardState(),
                PlayerBBoardState = gameState.PlayerBState.GetSerializedGameBoardState()
            });

            dbCtx.SaveChanges();
        }

        private static AppDbContext GetConnection()
        {
            var dbOptions = new DbContextOptionsBuilder<AppDbContext>().UseSqlServer(
                @"
                Server=barrel.itcollege.ee,1533;
                User Id=student;
                Password=Student.Bad.password.0;
                Database=raskil_db;
                MultipleActiveResultSets=true;
                ").Options;

            return new AppDbContext(dbOptions);
        }
    }
}