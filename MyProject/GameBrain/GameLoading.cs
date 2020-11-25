using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using DAL;
using Microsoft.EntityFrameworkCore;

namespace GameBrain
{
    public class GameLoading
    {
        public static GameOptions LoadLastGameOptions()
        {
            Console.Clear();
            Console.Write("Loading...");
            var dbOptions = new DbContextOptionsBuilder<AppDbContext>().UseSqlServer(
                @"
                Server=barrel.itcollege.ee,1533;
                User Id=student;
                Password=Student.Bad.password.0;
                Database=raskil_db;
                MultipleActiveResultSets=true;
                ").Options;
            using var dbCtx = new AppDbContext(dbOptions);

            // ReSharper disable once PossibleNullReferenceException
            var lastGameId = dbCtx.Games
                .OrderByDescending(g => g.GameId)
                .FirstOrDefault().GameId;

            var gameOptionId = dbCtx.Games
                .Where(g => g.GameId == lastGameId)
                .Select(x => x.GameOptionId)
                .FirstOrDefault();
            var boardWidth = dbCtx.GameOptions
                .Where(g => g.GameOptionId == gameOptionId)
                .Select(x => x.BoardWidth)
                .FirstOrDefault();
            var boardHeight = dbCtx.GameOptions
                .Where(g => g.GameOptionId == gameOptionId)
                .Select(x => x.BoardHeight)
                .FirstOrDefault();
            var shipsCanTouch = dbCtx.GameOptions
                .Where(g => g.GameOptionId == gameOptionId)
                .Select(x => x.EShipsCanTouch)
                .FirstOrDefault();
            var nextMoveAfterHit = dbCtx.GameOptions
                .Where(g => g.GameOptionId == gameOptionId)
                .Select(x => x.NextMoveAfterHit)
                .FirstOrDefault();
            var nextMoveByPlayerA = dbCtx.GameOptions
                .Where(g => g.GameOptionId == gameOptionId)
                .Select(x => x.NextMoveByPlayerA)
                .FirstOrDefault();

            var playerAId = dbCtx.Games
                .Where(g => g.GameId == lastGameId)
                .Select(x => x.PlayerAId)
                .FirstOrDefault();
            var playerAName = dbCtx.Players
                .Where(p => p.PlayerId == playerAId)
                .Select(x => x.Name)
                .FirstOrDefault() ?? "";
            var playerAType = dbCtx.Players
                .Where(p => p.PlayerId == playerAId)
                .Select(x => x.EPlayerType)
                .FirstOrDefault();
            var playerAShipsId = dbCtx.GameShips
                .Where(g => g.PlayerId == playerAId)
                .Select(x => x.GameShipId)
                .FirstOrDefault();
            var playerAShips = dbCtx.GameShips
                .Where(s => s.GameShipId == playerAShipsId);
            var shipsA = new List<Ship>();
            foreach (var playerAShip in playerAShips)
            {
                shipsA.Add(new Ship
                {
                    Name = playerAShip.Name,
                    Width = playerAShip.Width,
                    Hits = playerAShip.Hits,
                    CellState = playerAShip.ECellState
                });
            }

            var gameBoardA = dbCtx.PlayerBoardStates
                .OrderByDescending(x => x.PlayerBoardStateId)
                .Where(x => x.PlayerId == playerAId)
                .Select(x => x.GameBoardState)
                .FirstOrDefault();

            var firingBoardA = dbCtx.PlayerBoardStates
                .OrderByDescending(x => x.PlayerBoardStateId)
                .Where(x => x.PlayerId == playerAId)
                .Select(x => x.FiringBoardState)
                .FirstOrDefault();

            var playerBId = dbCtx.Games
                .Where(g => g.GameId == lastGameId)
                .Select(x => x.PlayerBId)
                .FirstOrDefault();
            var playerBName = dbCtx.Players
                .Where(p => p.PlayerId == playerBId)
                .Select(x => x.Name)
                .FirstOrDefault() ?? "";
            var playerBType = dbCtx.Players
                .Where(p => p.PlayerId == playerBId)
                .Select(x => x.EPlayerType)
                .FirstOrDefault();
            var playerBShipsId = dbCtx.GameShips
                .Where(g => g.PlayerId == playerBId)
                .Select(x => x.GameShipId)
                .FirstOrDefault();
            var playerBShips = dbCtx.GameShips
                .Where(s => s.GameShipId == playerBShipsId);
            var shipsB = new List<Ship>();
            foreach (var playerAShip in playerBShips)
            {
                shipsB.Add(new Ship
                {
                    Name = playerAShip.Name,
                    Width = playerAShip.Width,
                    Hits = playerAShip.Hits,
                    CellState = playerAShip.ECellState
                });
            }

            // Should get with include!
            var gameBoardB = dbCtx.PlayerBoardStates
                .OrderByDescending(x => x.PlayerBoardStateId)
                .Where(x => x.PlayerId == playerBId)
                .Select(x => x.GameBoardState)
                .FirstOrDefault();
            var firingBoardB = dbCtx.PlayerBoardStates
                .OrderByDescending(x => x.PlayerBoardStateId)
                .Where(x => x.PlayerId == playerBId)
                .Select(x => x.FiringBoardState)
                .FirstOrDefault();

            var playerA = new Player(playerAName)
            {
                Ships = shipsA,
                PlayerType = playerAType,
                PlayerBoard = new GameBoard(boardWidth, boardHeight),
                FiringBoard = new GameBoard(boardWidth, boardHeight)
            };

            var stateAGame = JsonSerializer.Deserialize<GameBoardState>(gameBoardA);

            for (var x = 0; x < boardWidth; x++)
            {
                for (var y = 0; y < boardHeight; y++)
                {
                    playerA.GetPlayerBoard()[x, y] = stateAGame!.Board[x][y];
                }
            }

            var stateAFiring = JsonSerializer.Deserialize<GameBoardState>(firingBoardA);
            for (var x = 0; x < boardWidth; x++)
            {
                for (var y = 0; y < boardHeight; y++)
                {
                    playerA.GetFiringBoard()[x, y] = stateAFiring!.Board[x][y];
                }
            }


            var playerB = new Player(playerBName)
            {
                Ships = shipsB,
                PlayerType = playerBType,
                PlayerBoard = new GameBoard(boardWidth, boardHeight),
                FiringBoard = new GameBoard(boardWidth, boardHeight)
            };

            var stateBGame = JsonSerializer.Deserialize<GameBoardState>(gameBoardB);
            for (var x = 0; x < boardWidth; x++)
            {
                for (var y = 0; y < boardHeight; y++)
                {
                    playerB.GetPlayerBoard()[x, y] = stateBGame!.Board[x][y];
                }
            }

            var stateBFiring = JsonSerializer.Deserialize<GameBoardState>(firingBoardB);
            for (var x = 0; x < boardWidth; x++)
            {
                for (var y = 0; y < boardHeight; y++)
                {
                    playerB.GetFiringBoard()[x, y] = stateBFiring!.Board[x][y];
                }
            }

            // Should not have "shipsA"!!!!
            return new GameOptions(boardWidth, boardHeight, playerA, playerB, shipsCanTouch, shipsA, nextMoveAfterHit)
            {
                // BoardWidth = boardWidth,
                // BoardHeight = boardHeight,
                // ShipsCanTouch = shipsCanTouch,
                // NextMoveAfterHit = nextMoveAfterHit,
                // NextMoveByPlayerA = nextMoveByPlayerA,
                // PlayerA = playerA,
                // PlayerB = playerB,
            };
        }
    }
}