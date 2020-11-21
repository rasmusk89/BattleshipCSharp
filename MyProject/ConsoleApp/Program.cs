using System;
using System.Collections.Generic;
using System.Linq;
using DAL;
using Domain;
using Domain.Enums;
using GameBrain;
using MenuSystem;
using Microsoft.EntityFrameworkCore;
using EPlayerType = Domain.Enums.EPlayerType;
using EShipsCanTouch = GameBrain.EShipsCanTouch;
using Game = GameBrain.Game;
using Ship = GameBrain.Ship;

namespace ConsoleApp
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("+---------------------------+\n" +
                              "|        RASMUS GAME        |\n" +
                              "+---------------------------+");
            Console.ForegroundColor = ConsoleColor.Cyan;

            var playerType = new Menu(MenuLevel.Level2Plus);
            playerType.AddMenuItem(new MenuItem(
                "Player VS Player",
                "1",
                Battleship));

            var gameType = new Menu(MenuLevel.Level1);
            gameType.AddMenuItem(new MenuItem(
                "Start Basic Game",
                "S",
                playerType.RunMenu
            ));

            gameType.AddMenuItem(new MenuItem(
                "Start Custom Game",
                "C",
                CustomBattleship
            ));

            gameType.AddMenuItem(new MenuItem(
                "Load Last Game",
                "L",
                LoadGame
            ));


            var menu = new Menu(MenuLevel.Level0);
            menu.AddMenuItem(new MenuItem("Battleship", "B", gameType.RunMenu));
            menu.AddMenuItem(new MenuItem("DB Test", "D", DbTest));
            menu.RunMenu();
        }

        private static string Battleship()
        {
            var options = new GameOptions();
            var game = new Game(options);
            game.PlayRound();
            return "";
        }

        private static string CustomBattleship()
        {
            Console.Write("Set board width: ");
            var width = int.Parse(Console.ReadLine());

            Console.Write("Set board height: ");
            var height = int.Parse(Console.ReadLine());

            Console.Write("Can ships touch? (Y/N): ");
            var canTouch = false || Console.ReadLine().ToLower() == "y";
            var shipsCanTouch = EShipsCanTouch.No;
            if (canTouch)
            {
                shipsCanTouch = EShipsCanTouch.Yes;
            }

            Console.Write("Set player A name: ");
            string playerAName = Console.ReadLine();

            Console.Write("Set player B name: ");
            string playerBName = Console.ReadLine();

            List<Ship> ships = new List<Ship>();

            Console.WriteLine("Enter number of Patrol (1x1) ships: ");
            string patrol = Console.ReadLine() ?? "";
            var nPatrol = int.Parse(patrol);

            Console.WriteLine("Enter number of Cruiser (2x1) ships: ");
            string cruiser = Console.ReadLine() ?? "";
            var nCruiser = int.Parse(cruiser);

            Console.WriteLine("Enter number of Submarine (3x1) ships: ");
            string submarine = Console.ReadLine() ?? "";
            var nSubmarine = int.Parse(submarine);

            Console.WriteLine("Enter number of Battleship (4x1) ships: ");
            string battleship = Console.ReadLine() ?? "";
            var nBattleship = int.Parse(battleship);

            Console.WriteLine("Enter number of Carrier (5x1) ships: ");
            string carrier = Console.ReadLine() ?? "";
            var nCarrier = int.Parse(carrier);

            for (var i = 0; i < nPatrol; i++)
            {
                ships.Add(new Ship(1));
            }

            for (var i = 0; i < nCruiser; i++)
            {
                ships.Add(new Ship(2));
            }

            for (var i = 0; i < nSubmarine; i++)
            {
                ships.Add(new Ship(3));
            }

            for (var i = 0; i < nBattleship; i++)
            {
                ships.Add(new Ship(4));
            }

            for (var i = 0; i < nCarrier; i++)
            {
                ships.Add(new Ship(5));
            }

            var options = new GameOptions(width, height, playerAName, playerBName, shipsCanTouch, ships, false);

            var game = new Game(options);
            game.PlayRound();
            return "";
        }

        private static string LoadGame()
        {
            var options = new GameOptions();
            var game = new Game(options);
            game.LoadGameAction();
            return "";
        }

        private static string DefaultMenuAction()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("\nNot implemented yet!");
            Console.ForegroundColor = ConsoleColor.Cyan;

            return "";
        }

        private static string DbTest()
        {
            var mssql = @"
                Server=barrel.itcollege.ee,1533;
                User Id=student;
                Password=Student.Bad.password.0;
                Database=raskil_db;
                MultipleActiveResultSets=true;
                ";


            var dbOptions = new DbContextOptionsBuilder<AppDbContext>().UseSqlServer(mssql).Options;
            using var dbCtx = new AppDbContext(dbOptions);
            Console.WriteLine("Deleting database..");
            dbCtx.Database.EnsureDeleted();
            Console.WriteLine("Migrating database..");
            dbCtx.Database.Migrate();
            Console.WriteLine("Adding data to database..");


            var playerA = new Domain.Player()
            {
                Name = "Rasmus",
                EPlayerType = EPlayerType.Human
            };
            var playerB = new Domain.Player()
            {
                Name = "Anette",
                EPlayerType = EPlayerType.Human
            };
            var game = new Domain.Game()
            {
            };

            game.PlayerA = playerA;
            game.PlayerB = playerB;

            var gameOption = new GameOption
            {
                Name = "Standard 10x10",
                BoardWidth = 10,
                BoardHeight = 10,
                // EShipsCanTouch = EShipsCanTouch.No,
                NextMoveAfterHit = ENextMoveAfterHit.OtherPlayer
            };

            game.GameOption = gameOption;

            // var ship1 = new Domain.Ship()
            // {
            //     Name = "Boat",
            //     Width = 1,
            // };
            //
            // var ship2 = new Domain.Ship()
            // {
            //     Name = "Cruiser",
            //     Width = 2,
            // };
            //
            // var ship3 = new Domain.Ship()
            // {
            //     Name = "Battleship",
            //     Width = 3,
            // };
            //
            // var ship4 = new Domain.Ship()
            // {
            //     Name = "Submarine",
            //     Width = 4,
            // };
            //
            // var ship5 = new Domain.Ship()
            // {
            //     Name = "Carrier",
            //     Width = 5,
            // };
            //
            // var ships = new List<Domain.Ship> {ship1, ship2, ship3, ship4, ship5};
            //
            // var gameOptionShips = ships
            //     .Select(ship => new GameOptionShip {Amount = 1, GameOption = gameOption, Ship = ship})
            //     .ToList();
            //
            // foreach (var gameOptionShip in gameOptionShips)
            // {
            //     dbCtx.GameOptionShips.Add(gameOptionShip);
            // }
            
            dbCtx.Games.Add(game);
            dbCtx.SaveChanges();

            return "";
        }
    }
}