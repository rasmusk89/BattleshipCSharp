using System.Collections.Generic;
using System.Linq;
using DAL;
using Microsoft.EntityFrameworkCore;

namespace GameBrain
{
    public class GameLoading
    {
        public static Game LoadGameById(int id)
        {

            using var dbCtx = GetConnection();

            var gameById = dbCtx.Games.Where(i => i.GameId == id)
                .Include(x => x.GameStates)
                .Include(option => option.GameOption)
                .Include(player => player.PlayerA)
                .Include(player => player.PlayerB)
                .First();

            var state = dbCtx.GameStates.OrderByDescending(x => x.GameStateId)
                .First(i => i.GameId == gameById.GameId);

            var numberOfShips = gameById.GameOption.NumberOfShips;

            var playerAShips = dbCtx.GameShips
                .Where(i => i.PlayerId == gameById.PlayerAId).OrderByDescending(x => x.GameShipId).Take(numberOfShips)
                .ToList()
                .Select(gameShip => new Ship()
                {
                    Name = gameShip.Name,
                    CellState = gameShip.ECellState,
                    Hits = gameShip.Hits,
                    Width = gameShip.Width
                })
                .ToList();

            var playerBShips = dbCtx.GameShips
                .Where(i => i.PlayerId == gameById.PlayerBId).OrderByDescending(x => x.GameShipId).Take(numberOfShips)
                .ToList()
                .Select(gameShip => new Ship()
                {
                    Name = gameShip.Name,
                    CellState = gameShip.ECellState,
                    Hits = gameShip.Hits,
                    Width = gameShip.Width
                })
                .ToList();

            var boardA = state.PlayerABoardState;
            var boardB = state.PlayerBBoardState;

            var playerA = new Player
            {
                Name = gameById.PlayerA.Name,
                PlayerType = gameById.PlayerA.PlayerType,
                Ships = playerAShips,
                GameBoard = new GameBoard(gameById.GameOption.BoardWidth, gameById.GameOption.BoardHeight)
            };
            playerA.SetGameBoardStateFromJsonString(boardA!);

            var playerB = new Player
            {
                Name = gameById.PlayerB.Name,
                PlayerType = gameById.PlayerB.PlayerType,
                Ships = playerBShips,
                GameBoard = new GameBoard(gameById.GameOption.BoardWidth, gameById.GameOption.BoardHeight)
            };
            playerB.SetGameBoardStateFromJsonString(boardB!);

            var options = new GameOptions
            {
                BoardWidth = gameById.GameOption.BoardWidth,
                BoardHeight = gameById.GameOption.BoardHeight,
                NextMoveAfterHit = gameById.GameOption.NextMoveAfterHit,
                ShipsCanTouch = gameById.GameOption.EShipsCanTouch,
            };

            var game = new Game(options)
            {
                PlayerA = playerA,
                PlayerB = playerB,
                NextMoveByPlayerA = state.NextMoveByPlayerA
            };

            return game;
        }

        public static IEnumerable<(int id, string desc)> GetListOfAllGames()
        {
            using var dbCtx = GetConnection();

            var games = dbCtx.Games.OrderByDescending(i => i.GameId);

            var allGames = new List<(int id, string desc)>();

            foreach (var game in games)
            {
                allGames.Add((game.GameId, game.Description));
            }

            return allGames;
        }

        public static Game LoadLastGame()
        {
            using var dbCtx = GetConnection();

            var lastGame = dbCtx.Games.OrderByDescending(x => x.GameId)
                .Include(x => x.GameStates)
                .Include(option => option.GameOption)
                .Include(player => player.PlayerA)
                .Include(player => player.PlayerB)
                .First();

            var numberOfShips = lastGame.GameOption.NumberOfShips;

            var state = dbCtx.GameStates.OrderByDescending(x => x.GameStateId)
                .First(id => id.GameId == lastGame.GameId);

            var playerAShips = dbCtx.GameShips
                .Where(id => id.PlayerId == lastGame.PlayerAId).OrderByDescending(x => x.GameShipId).Take(numberOfShips)
                .ToList()
                .Select(gameShip => new Ship()
                {
                    Name = gameShip.Name,
                    CellState = gameShip.ECellState,
                    Hits = gameShip.Hits,
                    Width = gameShip.Width
                })
                .ToList();

            var playerBShips = dbCtx.GameShips
                .Where(id => id.PlayerId == lastGame.PlayerBId).OrderByDescending(x => x.GameShipId).Take(numberOfShips)
                .ToList()
                .Select(gameShip => new Ship()
                {
                    Name = gameShip.Name,
                    CellState = gameShip.ECellState,
                    Hits = gameShip.Hits,
                    Width = gameShip.Width,
                })
                .ToList();

            var boardA = state.PlayerABoardState;
            var boardB = state.PlayerBBoardState;

            var playerA = new Player
            {
                Name = lastGame.PlayerA.Name,
                PlayerType = lastGame.PlayerA.PlayerType,
                Ships = playerAShips,
                GameBoard = new GameBoard(lastGame.GameOption.BoardWidth, lastGame.GameOption.BoardHeight)
            };
            playerA.SetGameBoardStateFromJsonString(boardA!);

            var playerB = new Player
            {
                Name = lastGame.PlayerB.Name,
                PlayerType = lastGame.PlayerB.PlayerType,
                Ships = playerBShips,
                GameBoard = new GameBoard(lastGame.GameOption.BoardWidth, lastGame.GameOption.BoardHeight)
            };
            playerB.SetGameBoardStateFromJsonString(boardB!);

            var options = new GameOptions
            {
                BoardWidth = lastGame.GameOption.BoardWidth,
                BoardHeight = lastGame.GameOption.BoardHeight,
                NextMoveAfterHit = lastGame.GameOption.NextMoveAfterHit,
                ShipsCanTouch = lastGame.GameOption.EShipsCanTouch,
            };

            var game = new Game(options)
            {
                PlayerA = playerA,
                PlayerB = playerB,
                NextMoveByPlayerA = state.NextMoveByPlayerA
            };

            return game;
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