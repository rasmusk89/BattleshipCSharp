using System.Collections.Generic;
using System.Linq;
using DAL;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace GameBrain
{
    public class GameLoading
    {
         public GameOptions LoadLastGameOptions()
        {
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
                .Where(x => x.PlayerId == playerAId)
                .Select(x => x.GameBoardState)
                .FirstOrDefault();
                
            var firingBoardA = dbCtx.PlayerBoardStates
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
            var gameBoardB = dbCtx.PlayerBoardStates
                    .Where(x => x.PlayerId == playerBId)
                    .Select(x => x.GameBoardState)
                    .FirstOrDefault();
            var firingBoardB = dbCtx.PlayerBoardStates
                    .Where(x => x.PlayerId == playerBId)
                    .Select(x => x.FiringBoardState)
                    .FirstOrDefault();

            var playerA = new Player(playerAName, boardWidth, boardHeight, shipsCanTouch)
            {
                Ships = shipsA,
                PlayerType = playerAType,
                PlayerBoard = new GameBoard(boardWidth, boardHeight),
                OpponentBoard = new FiringBoard(boardWidth, boardHeight)
            };
            
            var stateAGame = JsonSerializer.Deserialize<GameBoardState>(gameBoardA);
            
            for (var x = 0; x < boardWidth; x++)
            {
                for (var y = 0; y < boardHeight; y++)
                {
                    playerA.PlayerBoard.Board[x, y] = stateAGame!.PlayerBoard[x][y];
                }
            }
            
            var stateAFiring = JsonSerializer.Deserialize<FiringBoardState>(firingBoardA);
            for (var x = 0; x < boardWidth; x++)
            {
                for (var y = 0; y < boardHeight; y++)
                {
                    playerA.OpponentBoard.Board[x, y] = stateAFiring!.OpponentBoard[x][y];
                }
            }
            

            var playerB = new Player(playerBName, boardWidth, boardHeight, shipsCanTouch)
            {
                Ships = shipsB,
                PlayerType = playerBType,
                PlayerBoard = new GameBoard(boardWidth, boardHeight),
                OpponentBoard = new FiringBoard(boardWidth, boardHeight)
            };
            
            var stateBGame = JsonSerializer.Deserialize<GameBoardState>(gameBoardB);
            for (var x = 0; x < boardWidth; x++)
            {
                for (var y = 0; y < boardHeight; y++)
                {
                    playerB.PlayerBoard.Board[x, y] = stateBGame!.PlayerBoard[x][y];
                }
            }
            var stateBFiring = JsonSerializer.Deserialize<FiringBoardState>(firingBoardB);
            for (var x = 0; x < boardWidth; x++)
            {
                for (var y = 0; y < boardHeight; y++)
                {
                    playerB.OpponentBoard.Board[x, y] = stateBFiring!.OpponentBoard[x][y];
                }
            }
            return new GameOptions
            {
                BoardWidth = boardWidth,
                BoardHeight = boardHeight,
                ShipsCanTouch = shipsCanTouch,
                NextMoveAfterHit = nextMoveAfterHit,
                NextMoveByPlayerA = nextMoveByPlayerA,
                PlayerA = playerA,
                PlayerB = playerB,
            };
        }
    }
}