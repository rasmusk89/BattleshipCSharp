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
        
        public static void InitialSave(GameState gameState)
        {
            using var dbCtx = GetConnection();
            Console.Write("Saving...");
            dbCtx.Database.EnsureDeleted();
            dbCtx.Database.Migrate();

            var playerOne = gameState.PlayerAState;
            var playerTwo = gameState.PlayerBState;
            
            var playerA = new Domain.Player
            {
                Name = playerOne.GetName(),
                EPlayerType = playerOne.GetPlayerType(),
                GameShips = new List<GameShip>(),
                PlayerBoardStates = new List<PlayerBoardState>(),
            };
            var playerB = new Domain.Player
            {
                Name = playerTwo.GetName(),
                EPlayerType = playerTwo.GetPlayerType(),
                GameShips = new List<GameShip>(),
                PlayerBoardStates = new List<PlayerBoardState>(),
            };

            var ships = gameState.ShipsState;
            
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

            var playerABoardState = new PlayerBoardState
            {
                CreatedAt = DateTime.Now,
                GameBoardState = playerOne.GetSerializedGameBoardState(),
                Player = playerA
            };
            
            var playerBBoardState = new PlayerBoardState
            {
                CreatedAt = DateTime.Now,
                GameBoardState = playerTwo.GetSerializedGameBoardState(),
                Player = playerB
            };
            
            playerA.GameShips = playerAShips;
            playerA.PlayerBoardStates.Add(playerABoardState);
            playerB.GameShips = playerBShips;
            playerB.PlayerBoardStates.Add(playerBBoardState);
            
            var gameOptions = new GameOption
            {
                Name = DateTime.Now.ToString(CultureInfo.CurrentCulture),
                BoardWidth = gameState.BoardWidthState,
                BoardHeight = gameState.BoardHeightState,
                EShipsCanTouch = gameState.ShipsCanTouchState,
                NextMoveAfterHit = gameState.GameOptions.GetNextMoveAfterHit(),
            };

            var newGame = new Domain.Game
            {
                Description = $"{playerA.Name}_{playerB.Name}_{DateTime.Now}",
                GameOption = gameOptions,
                NextMoveByPlayerA = gameState.NextMoveByPlayerAState,
                PlayerA = playerA,
                PlayerB = playerB
            };
            
            dbCtx.Games.Add(newGame);
            dbCtx.SaveChanges();
        }
    }
}